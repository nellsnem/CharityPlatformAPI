document.addEventListener('DOMContentLoaded', async () => {
    const userId = localStorage.getItem('userId');
    const userName = localStorage.getItem('userName');

    if (!userId) {
        window.location.href = 'auth.html';
        return;
    }

    // Завантажуємо дані користувача
    try {
        const response = await fetch(`/api/User/${userId}`);
        if (response.ok) {
            const userData = await response.json();
            document.getElementById('user-name-display').textContent = userData.fullName;
            document.getElementById('user-phone-display').textContent = userData.phone;
        } else {
            document.getElementById('user-name-display').textContent = userName;
            document.getElementById('user-phone-display').textContent = "Не вдалося завантажити";
        }
    } catch (error) {
        document.getElementById('user-name-display').textContent = userName;
    }

    document.getElementById('logout-btn').addEventListener('click', (e) => {
        e.preventDefault();
        localStorage.clear();
        window.location.href = 'index.html';
    });

    // --- Елементи DOM ---
    const btnBeneficiary = document.getElementById('btn-beneficiary');
    const panelBeneficiary = document.getElementById('beneficiary-panel');
    const dashboardBenView = document.getElementById('beneficiary-dashboard-view');
    const formBenView = document.getElementById('beneficiary-form-view');
    const successBenView = document.getElementById('beneficiary-success-view');
    const requestsBody = document.getElementById('beneficiary-requests-body');
    const requestForm = document.getElementById('request-form');

    const btnNewRequest = document.getElementById('btn-new-request');
    const btnCancelRequest = document.getElementById('btn-cancel-request');
    const btnBackToDashboard = document.getElementById('btn-back-to-dashboard');

    const btnVolunteer = document.getElementById('btn-volunteer');
    const panelVolunteer = document.getElementById('volunteer-panel');


    // ==========================================
    // ЛОГІКА БЕНЕФІЦІАРА
    // ==========================================

    async function loadBeneficiaryDashboard() {
        btnBeneficiary.style.display = 'none';
        panelBeneficiary.style.display = 'block';

        dashboardBenView.style.display = 'block';
        formBenView.style.display = 'none';
        successBenView.style.display = 'none';

        requestsBody.innerHTML = '<tr><td colspan="3">Завантаження...</td></tr>';

        try {
            // 1. Перевіряємо, чи є користувач бенефіціаром
            const checkBenResponse = await fetch(`/api/Beneficiary/check/${userId}`);
            if (!checkBenResponse.ok) throw new Error("Помилка");
            const checkBenData = await checkBenResponse.json();

            if (!checkBenData.isBeneficiary) {
                requestsBody.innerHTML = '<tr><td colspan="3">У вас ще немає запитів.</td></tr>';
                return;
            }

            const beneficiaryId = checkBenData.beneficiaryData.beneficiaryId;

            // 2. Отримуємо всі запити
            const reqResponse = await fetch('/api/HelpRequest');
            if (!reqResponse.ok) throw new Error("Помилка");
            const allRequests = await reqResponse.json();

            const myRequests = allRequests.filter(r => r.beneficiaryId === beneficiaryId);

            if (myRequests.length === 0) {
                requestsBody.innerHTML = '<tr><td colspan="3">У вас ще немає запитів.</td></tr>';
                return;
            }

            requestsBody.innerHTML = '';
            myRequests.forEach(req => {
                const statusLower = (req.status || '').toLowerCase();
                const statusColor = (statusLower === 'в обробці' || statusLower === 'нове') ? '#d34c3d' : '#3b7a57';

                // === РОЗУМНИЙ ВИВІД СТАТУСУ З КНОПКОЮ ===
                let statusHtml = `<span style="color: ${statusColor}; font-weight: bold;">${req.status}</span>`;

                // Перевіряємо, чи статус "схвалено" і чи є ID проєкту з бекенду
                if (statusLower === 'схвалено' && req.projectId) {
                    statusHtml = `
                        <span style="color: #3b7a57; font-weight: bold;">Схвалено </span><br>
                        <a href="project.html?id=${req.projectId}" style="display: inline-block; margin-top: 5px; font-size: 0.85rem; padding: 5px 10px; background-color: #3b7a57; color: #fbf3db; text-decoration: none; border-radius: 4px; font-weight: bold;">
                             Відкрити збір
                        </a>
                    `;
                } else if (statusLower === 'відхилено') {
                    statusHtml = `<span style="color: #555; font-weight: bold;">Відхилено </span>`;
                }

                requestsBody.innerHTML += `
                    <tr>
                        <td style="vertical-align: middle;">№${req.requestId}</td>
                        <td style="word-break: break-word; white-space: normal;">${req.needDescription}</td>
                        <td style="vertical-align: middle;">${statusHtml}</td>
                    </tr>
                `;
            });

        } catch (error) {
            console.error(error);
            requestsBody.innerHTML = '<tr><td colspan="3">Помилка завантаження даних.</td></tr>';
        }
    }

    btnBeneficiary.addEventListener('click', () => {
        loadBeneficiaryDashboard();
    });

    btnNewRequest.addEventListener('click', () => {
        dashboardBenView.style.display = 'none';
        formBenView.style.display = 'block';
    });

    btnCancelRequest.addEventListener('click', () => {
        clearErrors();
        requestForm.reset();
        formBenView.style.display = 'none';
        dashboardBenView.style.display = 'block';
    });

    btnBackToDashboard.addEventListener('click', () => {
        requestForm.reset();
        loadBeneficiaryDashboard();
    });

    // --- ФУНКЦІЇ ДЛЯ ВИВОДУ ПОМИЛОК ---
    function showError(elementId, message) {
        const errorDiv = document.getElementById(elementId);
        if (errorDiv) {
            errorDiv.textContent = message;
            errorDiv.style.display = 'block';
        }
    }

    function clearErrors() {
        document.querySelectorAll('.field-error, .error-message').forEach(el => {
            el.style.display = 'none';
            el.textContent = '';
        });
    }

    // Відправка форми Бенефіціара
    if (requestForm) {
        requestForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearErrors();

            let hasErrors = false;

            const passport = document.getElementById('req-passport').value.trim();
            const desc = document.getElementById('req-desc').value.trim();
            const address = document.getElementById('req-address').value.trim();
            const fileInput = document.getElementById('req-document');

            // ВАЛІДАЦІЯ
            const passportRegex = /^([А-ЯІЇЄҐA-Z]{2}\s?\d{6}|\d{9})$/i;
            if (!passport) {
                showError('error-passport', 'Номер паспорта є обов\'язковим.');
                hasErrors = true;
            } else if (!passportRegex.test(passport)) {
                showError('error-passport', 'Некоректний формат (приклад: ВВ 123456 або 123456789).');
                hasErrors = true;
            }
            if (!desc) {
                showError('error-desc', 'Будь ласка, опишіть вашу потребу.');
                hasErrors = true;
            }
            if (!address) {
                showError('error-address', 'Будь ласка, вкажіть адресу доставки.');
                hasErrors = true;
            }
            if (fileInput.files.length === 0) {
                showError('error-document', 'Будь ласка, обов\'язково прикріпіть фото документа.');
                hasErrors = true;
            }

            if (hasErrors) return;
            const fileName = fileInput.files[0].name;

            try {
                // КРОК 1: Перевіряємо / Створюємо Бенефіціара
                let beneficiaryId = null;
                const checkBenResponse = await fetch(`/api/Beneficiary/check/${userId}`);
                if (checkBenResponse.ok) {
                    const checkBenData = await checkBenResponse.json();
                    if (checkBenData.isBeneficiary) {
                        beneficiaryId = checkBenData.beneficiaryData.beneficiaryId;
                    }
                }

                if (!beneficiaryId) {
                    const createBenResponse = await fetch('/api/Beneficiary', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            UserId: parseInt(userId),
                            PassportNumber: passport
                        })
                    });

                    if (!createBenResponse.ok) {
                        const errText = await createBenResponse.text();
                        showError('server-error', `Помилка реєстрації: ${errText || 'Можливо, цей паспорт вже є в базі.'}`);
                        return;
                    }

                    const reCheckBenResponse = await fetch(`/api/Beneficiary/check/${userId}`);
                    const reCheckBenData = await reCheckBenResponse.json();
                    beneficiaryId = reCheckBenData.beneficiaryData.beneficiaryId;
                }

                // КРОК 2: Відправляємо ЗАПИТ_НА_ДОПОМОГУ
                const requestData = {
                    BeneficiaryId: beneficiaryId,
                    NeedDescription: desc,
                    DocumentPath: fileName,
                    DeliveryAddress: address
                };

                const response = await fetch('/api/HelpRequest', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(requestData)
                });

                if (!response.ok) {
                    const textResponse = await response.text();
                    showError('server-error', `Помилка створення запиту: ${textResponse}`);
                    return;
                }

                formBenView.style.display = 'none';
                successBenView.style.display = 'block';

            } catch (error) {
                console.error(error);
                showError('server-error', "Помилка з'єднання з сервером.");
            }
        });
    }

    // ==========================================
    // ЛОГІКА ВОЛОНТЕРА
    // ==========================================

    async function loadVolunteerDashboard(currentVolunteerId) {
        btnVolunteer.style.display = 'none';
        panelVolunteer.style.display = 'block';

        const tbody = document.getElementById('volunteer-stats-body');
        const eventsList = document.getElementById('volunteer-events-list');

        tbody.innerHTML = '<tr><td colspan="2">Завантаження...</td></tr>';
        eventsList.innerHTML = '<li>Завантаження...</li>';

        try {
            const response = await fetch('/api/VolunteerParticipation');
            if (!response.ok) throw new Error('Помилка сервера');
            const allParticipations = await response.json();

            const myParticipations = allParticipations.filter(p => p.volunteerId === currentVolunteerId);

            const volunteerTotals = {};
            allParticipations.forEach(p => {
                if (!volunteerTotals[p.volunteerId]) {
                    volunteerTotals[p.volunteerId] = 0;
                }
                volunteerTotals[p.volunteerId] += p.hoursWorked;
            });

            const myTotalHours = volunteerTotals[currentVolunteerId] || 0;
            const sortedTotals = Object.values(volunteerTotals).sort((a, b) => b - a);
            let myRank = 1;
            if (sortedTotals.length > 0 && myTotalHours > 0) {
                myRank = sortedTotals.indexOf(myTotalHours) + 1;
            } else if (myTotalHours === 0) {
                myRank = sortedTotals.length > 0 ? sortedTotals.length : 1;
            }

            document.getElementById('total-hours-display').textContent = myTotalHours;
            document.getElementById('rank-display').textContent = myRank === 1 ? ' 1 місце' : myRank + ' місце';

            tbody.innerHTML = '';
            eventsList.innerHTML = '';

            let hasCompleted = false;
            let hasUpcoming = false;

            myParticipations.forEach(p => {
                if (p.hoursWorked > 0) {
                    hasCompleted = true;
                    tbody.innerHTML += `<tr><td>Захід №${p.eventId}</td><td>${p.hoursWorked} год.</td></tr>`;
                }

                if (p.statusPresence === 'зареєстровано') {
                    hasUpcoming = true;
                    eventsList.innerHTML += `<li>Захід №${p.eventId} — <span>Очікується</span></li>`;
                }
            });

            if (!hasCompleted) tbody.innerHTML = '<tr><td colspan="2">У вас ще немає відпрацьованих годин.</td></tr>';
            if (!hasUpcoming) eventsList.innerHTML = '<li>Немає запланованих заходів.</li>';

        } catch (error) {
            console.error(error);
            tbody.innerHTML = '<tr><td colspan="2">Помилка завантаження даних.</td></tr>';
            eventsList.innerHTML = '<li>Помилка завантаження даних.</li>';
        }
    }

    btnVolunteer.addEventListener('click', async () => {
        try {
            const response = await fetch('/api/Volunteer', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ UserId: parseInt(userId) })
            });

            if (!response.ok) return;

            const checkResponse = await fetch(`/api/Volunteer/check/${userId}`);
            const checkData = await checkResponse.json();
            if (checkData.isVolunteer) loadVolunteerDashboard(checkData.volunteerData.volunteerId);
        } catch (error) {
            console.error("Помилка з'єднання з сервером.");
        }
    });

    // ==========================================
    // ПЕРЕВІРКА СТАТУСІВ ПРИ ЗАВАНТАЖЕННІ 
    // ==========================================
    try {
        const checkVolResponse = await fetch(`/api/Volunteer/check/${userId}`);
        if (checkVolResponse.ok) {
            const checkVolData = await checkVolResponse.json();
            if (checkVolData.isVolunteer) {
                loadVolunteerDashboard(checkVolData.volunteerData.volunteerId);
            }
        }
    } catch (error) {
        console.error("Не вдалося перевірити статус волонтера:", error);
    }

    try {
        const checkBenResponse = await fetch(`/api/Beneficiary/check/${userId}`);
        if (checkBenResponse.ok) {
            const checkBenData = await checkBenResponse.json();
            if (checkBenData.isBeneficiary) {
                loadBeneficiaryDashboard();
            }
        }
    } catch (error) {
        console.error("Не вдалося перевірити статус бенефіціара:", error);
    }
});