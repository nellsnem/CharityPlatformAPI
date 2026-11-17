document.addEventListener('DOMContentLoaded', async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const projectId = urlParams.get('id');

    if (!projectId) {
        document.getElementById('loading-project').textContent = "Помилка: Проєкт не знайдено (немає ID).";
        return;
    }

    let currentCollected = 0;
    let currentBudget = 0;
    const FINANCE_API = '/api/Finance';

    // Завантажуємо дані проєкту
    try {
        const response = await fetch(`/api/Project/${projectId}`);
        if (!response.ok) throw new Error("Проєкт не знайдено");
        const project = await response.json();

        currentBudget = project.approvedBudget;

        try {
            const financeRes = await fetch(FINANCE_API);
            if (financeRes.ok) {
                const allTransactions = await financeRes.json();
                const projTransactions = allTransactions.filter(t =>
                    (t.projectId == projectId || t.ProjectId == projectId) &&
                    ((t.type && t.type.toLowerCase() === "надходження") || (t.Type && t.Type.toLowerCase() === "надходження"))
                );
                currentCollected = projTransactions.reduce((sum, t) => sum + (t.amount || t.Amount || 0), 0);
            }
        } catch (e) {
            console.log("Не вдалося завантажити транзакції");
        }

        let progressPercent = 0;
        if (currentBudget > 0) {
            progressPercent = (currentCollected / currentBudget) * 100;
            if (progressPercent > 100) progressPercent = 100;
        }

        // ===  ШУКАЄМО ВОЛОНТЕРСЬКИЙ ЗАХІД ДЛЯ ЦЬОГО ПРОЄКТУ ===
        try {
            const eventRes = await fetch(`/api/Event/project/${projectId}`);
            if (eventRes.ok) {
                const events = await eventRes.json();
                if (events && events.length > 0) {
                    const ev = events[0];

                    document.getElementById('event-info-title').textContent = ev.title;

                    const evDate = new Date(ev.eventDate);
                    document.getElementById('event-info-date').textContent = evDate.toLocaleString('uk-UA', {
                        year: 'numeric', month: 'long', day: 'numeric',
                        hour: '2-digit', minute: '2-digit'
                    });

                    document.getElementById('event-info-location').textContent = ev.location;

                    // 1. Витягуємо кількість місць з опису 
                    let totalSpots = 0;
                    const spotsMatch = ev.description.match(/\[Кількість місць:\s*(\d+)\]/);
                    if (spotsMatch && spotsMatch[1]) {
                        totalSpots = parseInt(spotsMatch[1]);
                    }

                    let cleanDesc = ev.description.replace(/\[Кількість місць:.*\]\n?/, '');
                    document.getElementById('event-info-desc').textContent = cleanDesc;

                    document.getElementById('project-event-block').style.display = 'block';

                    // === ЛОГІКА ПРИЄДНАННЯ ДО ЗАХОДУ ===
                    const currentUserId = localStorage.getItem('userId'); // ID користувача на головному сайті
                    const btnJoin = document.getElementById('btn-join-event');
                    const msgJoin = document.getElementById('join-event-message');

                    // 2. Рахуємо скільки вже записалося на цей захід
                    let currentParticipantsCount = 0;
                    let isAlreadyRegistered = false;
                    let currentVolunteerId = null;

                    try {
                        const partRes = await fetch(`/api/VolunteerParticipation/event/${ev.eventId}`);
                        if (partRes.ok) {
                            const participants = await partRes.json();
                            currentParticipantsCount = participants.length;
                        }
                    } catch (e) { console.log("Учасників ще немає"); }

                    const availableSpots = totalSpots - currentParticipantsCount;
                    document.getElementById('event-info-spots').textContent = `${availableSpots > 0 ? availableSpots : 0} з ${totalSpots}`;

                    // 3. Перевірки доступу
                    if (!currentUserId) {
                        msgJoin.textContent = "Станьте волонтером, щоб приєднатися до заходу.";
                    } else {
                        // Перевіряємо, чи цей користувач є волонтером
                        try {
                            const volRes = await fetch(`/api/Volunteer/check/${currentUserId}`);
                            if (volRes.ok) {
                                const volData = await volRes.json();
                                if (!volData.isVolunteer) {
                                    msgJoin.textContent = "Тільки зареєстровані волонтери можуть брати участь. Подайте заявку у своєму кабінеті!";
                                } else {
                                    currentVolunteerId = volData.volunteerData.volunteerId;

                                    // Перевіряємо, чи він вже натискав кнопку раніше
                                    try {
                                        const checkRes = await fetch(`/api/VolunteerParticipation/event/${ev.eventId}`);
                                        if (checkRes.ok) {
                                            const parts = await checkRes.json();
                                            isAlreadyRegistered = parts.some(p => p.volunteerId === currentVolunteerId);
                                        }
                                    } catch (e) { }

                                    if (isAlreadyRegistered) {
                                        msgJoin.textContent = " Ви вже зареєстровані на цей захід!";
                                        msgJoin.style.color = "#3b7a57";
                                    } else if (availableSpots <= 0) {
                                        msgJoin.textContent = "На жаль, усі місця на цей захід вже зайняті.";
                                    } else {
                                        btnJoin.style.display = "inline-block";
                                    }
                                }
                            }
                        } catch (e) { console.log("Помилка перевірки статусу волонтера"); }
                    }

                    // 4. Обробка натискання на кнопку "Приєднатися"
                    btnJoin.addEventListener('click', async () => {
                        if (!confirm("Ви впевнені, що хочете приєднатися до цього заходу?")) return;

                        btnJoin.disabled = true;
                        btnJoin.textContent = "Реєстрація...";

                        try {
                            const payload = {
                                EventId: ev.eventId,
                                VolunteerId: currentVolunteerId,
                                EmployeeId: null 
                            };

                            const joinRes = await fetch('/api/VolunteerParticipation', {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify(payload)
                            });

                            if (joinRes.ok) {
                                btnJoin.style.display = 'none';
                                msgJoin.textContent = " Ви успішно зареєстровані на цей захід!";
                                msgJoin.style.color = "#3b7a57";
                                document.getElementById('event-info-spots').textContent = `${availableSpots - 1 > 0 ? availableSpots - 1 : 0} з ${totalSpots}`;
                            } else {
                                msgJoin.textContent = "Помилка при реєстрації. Спробуйте пізніше.";
                                btnJoin.disabled = false;
                                btnJoin.textContent = " Приєднатися до заходу";
                            }
                        } catch (e) {
                            msgJoin.textContent = "Помилка зв'язку з сервером.";
                            btnJoin.disabled = false;
                            btnJoin.textContent = " Приєднатися до заходу";
                        }
                    });
                }
            }
        } catch (eventError) {
            console.log("Заходів для проєкту не знайдено або помилка мережі");
        }

        document.getElementById('proj-title').textContent = project.title;
        document.getElementById('proj-status').textContent = `Статус: ${project.status}`;
        document.getElementById('proj-desc').textContent = project.description;
        document.getElementById('proj-start-date').textContent = project.startDate || "Не вказано";

        document.getElementById('proj-collected').textContent = currentCollected.toLocaleString('uk-UA');
        document.getElementById('proj-budget').textContent = currentBudget.toLocaleString('uk-UA');
        document.getElementById('proj-progress-fill').style.width = `${progressPercent}%`;

        document.getElementById('loading-project').style.display = 'none';
        document.getElementById('project-content').style.display = 'flex';

        // === БЛОКУВАННЯ ДОНАТІВ, ЯКЩО ЗБІР ЗАКРИТО ===
        const statusLower = (project.status || "").toLowerCase();
        if (statusLower === 'зібрано' || statusLower === 'завершено' || statusLower === 'захід' || progressPercent >= 100) {
            const donateForm = document.getElementById('donate-form');
            const closedView = document.getElementById('project-closed-view');

            if (donateForm && closedView) {
                donateForm.style.display = 'none';
                closedView.style.display = 'block';
            }
        }

    } catch (error) {
        document.getElementById('loading-project').textContent = "Не вдалося завантажити інформацію про проєкт.";
        console.error(error);
    }

    // --- ВАЛІДАЦІЯ ---
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

    // Обробка форми донату
    const donateForm = document.getElementById('donate-form');
    if (donateForm) {
        donateForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            clearErrors();

            let hasErrors = false;
            const rawAmount = document.getElementById('donate-amount').value;
            const purpose = document.getElementById('donate-purpose').value.trim();
            const card = document.getElementById('donate-card').value.trim();

            let amount = 0;
            if (!rawAmount || !/^\d+$/.test(rawAmount)) {
                showError('error-amount', 'Сума донату повинна бути цілим числом.');
                hasErrors = true;
            } else {
                amount = parseInt(rawAmount, 10);
                if (amount < 1) {
                    showError('error-amount', 'Мінімальна сума донату — 1 ₴.');
                    hasErrors = true;
                }
            }

            if (!card || card.replace(/\s/g, '').length < 16) {
                showError('error-card', 'Введіть коректний номер картки (мінімум 16 цифр).');
                hasErrors = true;
            }

            if (hasErrors) return;

            const btnSubmit = document.getElementById('btn-submit-donate');
            btnSubmit.textContent = "Обробка...";
            btnSubmit.disabled = true;

            try {
                // РЕАЛЬНИЙ ЗАПИС У БАЗУ ДАНИХ
                const transactionData = {
                    ProjectId: parseInt(projectId),
                    Amount: parseFloat(amount),
                    Type: "надходження",
                    Purpose: purpose,
                    TransactionDate: new Date().toISOString().split('T')[0]
                };

                const payResponse = await fetch(FINANCE_API, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(transactionData)
                });

                if (!payResponse.ok) {
                    const errText = await payResponse.text();
                    try {
                        const errJson = JSON.parse(errText);
                        throw new Error(errJson.message || "Помилка оплати на сервері");
                    } catch (parseError) {
                        throw new Error(errText || "Помилка оплати на сервері");
                    }
                }

                currentCollected += amount;

                let newProgress = 0;
                if (currentBudget > 0) {
                    newProgress = (currentCollected / currentBudget) * 100;
                    if (newProgress > 100) newProgress = 100;
                }

                document.getElementById('proj-collected').textContent = currentCollected.toLocaleString('uk-UA');
                document.getElementById('proj-progress-fill').style.width = `${newProgress}%`;

                const resultData = await payResponse.json();
                if (resultData.message && resultData.message.includes('зібрано')) {
                    document.getElementById('proj-status').textContent = "Статус: зібрано";
                }

                btnSubmit.textContent = "Зробити переказ";
                btnSubmit.disabled = false;
                donateForm.style.display = 'none';

                if (newProgress >= 100 || (resultData.message && resultData.message.includes('зібрано'))) {
                    const btnDonateAgain = document.getElementById('btn-donate-again');
                    if (btnDonateAgain) btnDonateAgain.style.display = 'none';
                }

                document.getElementById('donate-success-view').style.display = 'block';

            } catch (error) {
                btnSubmit.textContent = "Зробити переказ";
                btnSubmit.disabled = false;
                showError('server-error', `${error.message}`);
            }
        });
    }

    const btnDonateAgain = document.getElementById('btn-donate-again');
    if (btnDonateAgain) {
        btnDonateAgain.addEventListener('click', () => {
            document.getElementById('donate-success-view').style.display = 'none';
            donateForm.reset();
            document.getElementById('donate-purpose').value = "Благодійний внесок";
            donateForm.style.display = 'block';
        });
    }
});