document.addEventListener('DOMContentLoaded', async () => {
    // === 1. ПЕРЕВІРКА АВТОРИЗАЦІЇ ===
    const employeeId = localStorage.getItem('employeeId');
    const employeeName = localStorage.getItem('employeeName');
    let employeeRole = localStorage.getItem('employeeRole');

    if (!employeeId) {
        window.location.href = 'staff-login.html';
        return;
    }

    if (employeeRole) {
        const tempRole = employeeRole.toLowerCase().trim();
        if (tempRole.includes('бухгал') || tempRole.includes('бухгалетр')) {
            employeeRole = 'Бухгалтер';
            localStorage.setItem('employeeRole', employeeRole);
        } else if (tempRole.includes('менеджер')) {
            employeeRole = 'Менеджер проєктів';
            localStorage.setItem('employeeRole', employeeRole);
        } else if (tempRole.includes('координатор')) {
            employeeRole = 'Координатор волонтерів';
            localStorage.setItem('employeeRole', employeeRole);
        }
    }

    document.getElementById('staff-name').textContent = employeeName;
    document.getElementById('staff-role').textContent = employeeRole;

    document.getElementById('staff-logout').addEventListener('click', (e) => {
        e.preventDefault();
        localStorage.clear();
        window.location.href = 'index.html';
    });


    // === 2. РОЗПОДІЛ ДОСТУПУ ===
    const menuEmp = document.getElementById('menu-employees');
    const menuTrans = document.getElementById('menu-transactions');
    const menuContr = document.getElementById('menu-contracts');
    const menuProj = document.getElementById('menu-projects');
    const menuVol = document.getElementById('menu-volunteers'); 
    const menuActiveEvents = document.getElementById('menu-active-events'); 

    const tabEmp = document.getElementById('tab-employees');
    const tabTrans = document.getElementById('tab-transactions');
    const tabContr = document.getElementById('tab-contracts');
    const tabProj = document.getElementById('tab-projects');
    const tabVol = document.getElementById('tab-volunteers');
    const tabActiveEvents = document.getElementById('tab-active-events');

    const navEmp = document.getElementById('nav-employees');
    const navTrans = document.getElementById('nav-transactions');
    const navContr = document.getElementById('nav-contracts');
    const navProj = document.getElementById('nav-projects');
    const navVol = document.getElementById('nav-volunteers'); 
    const navActiveEvents = document.getElementById('nav-active-events');

    const pageTitle = document.getElementById('page-title');

    function switchTab(activeNav, activeTab, titleText) {
        document.querySelectorAll('.admin-nav a').forEach(a => a.classList.remove('active'));
        activeNav.classList.add('active');
        document.querySelectorAll('.tab-pane').forEach(tab => tab.style.display = 'none');
        activeTab.style.display = 'block';
        pageTitle.textContent = titleText;
    }

    const roleLower = (employeeRole || '').toLowerCase().trim();

    // 1. Спочатку  ХОВАЄМО ВСЕ
    if (menuEmp) menuEmp.style.display = 'none';
    if (menuProj) menuProj.style.display = 'none';
    if (menuTrans) menuTrans.style.display = 'none';
    if (menuContr) menuContr.style.display = 'none';
    if (menuVol) menuVol.style.display = 'none';
    if (menuActiveEvents) menuActiveEvents.style.display = 'none'; 

    // 2. Тепер ПОКАЗУЄМО тільки те, що дозволено
    if (roleLower.includes('менеджер')) {
        // Менеджер бачить ТІЛЬКИ проєкти
        if (menuProj) menuProj.style.display = 'block';
        switchTab(navProj, tabProj, 'Управління проєктами (Нові заявки)');
        loadProjects();
    }
    else if (roleLower.includes('бухгал')) {
        // Бухгалтер бачить Фінанси, Договори ТА Працівників
        if (menuEmp) menuEmp.style.display = 'block';
        if (menuTrans) menuTrans.style.display = 'block';
        if (menuContr) menuContr.style.display = 'block';
        switchTab(navTrans, tabTrans, 'Фінансові транзакції');
        loadTransactions();
        loadEmployees();
    }
    else if (roleLower.includes('координатор')) {
        // Координатор бачить Організацію заходів та Облік годин
        if (menuVol) menuVol.style.display = 'block';
        if (menuActiveEvents) menuActiveEvents.style.display = 'block';
        switchTab(navVol, tabVol, 'Організація волонтерських заходів');
        loadVolunteerProjects();
    }
    else {
        // Директор бачить все
        if (menuEmp) menuEmp.style.display = 'block';
        if (menuProj) menuProj.style.display = 'block';
        if (menuTrans) menuTrans.style.display = 'block';
        if (menuContr) menuContr.style.display = 'block';
        if (menuVol) menuVol.style.display = 'block';
        if (menuActiveEvents) menuActiveEvents.style.display = 'block';
        switchTab(navEmp, tabEmp, 'Управління персоналом');
        loadEmployees();
    }


    // Обробка кліків по меню
    if (navEmp) navEmp.addEventListener('click', (e) => { e.preventDefault(); switchTab(navEmp, tabEmp, 'Управління персоналом'); loadEmployees(); });
    if (navTrans) navTrans.addEventListener('click', (e) => { e.preventDefault(); switchTab(navTrans, tabTrans, 'Фінансові транзакції'); loadTransactions(); });
    if (navContr) navContr.addEventListener('click', (e) => { e.preventDefault(); switchTab(navContr, tabContr, 'Реєстр договорів та контрагентів'); loadSuppliers(); loadContracts(); });
    if (navProj) navProj.addEventListener('click', (e) => { e.preventDefault(); switchTab(navProj, tabProj, 'Нові проєкти'); loadProjects(); });
    if (navVol) navVol.addEventListener('click', (e) => { e.preventDefault(); switchTab(navVol, tabVol, 'Організація волонтерських заходів'); loadVolunteerProjects(); });
    if (navActiveEvents) navActiveEvents.addEventListener('click', (e) => { e.preventDefault(); switchTab(navActiveEvents, tabActiveEvents, 'Облік відпрацьованих годин'); loadActiveEvents(); });


    // =======================================================================
    // === 3. ВКЛАДКА 1: ПРАЦІВНИКИ ==========================================
    // =======================================================================
    async function loadEmployees() {
        const tbody = document.getElementById('employees-table-body');
        try {
            const response = await fetch('/api/Employee');
            if (!response.ok) throw new Error();
            const employees = await response.json();
            tbody.innerHTML = '';
            if (employees.length === 0) {
                tbody.innerHTML = '<tr><td colspan="5">Немає зареєстрованих працівників.</td></tr>';
                return;
            }
            employees.forEach(emp => {
                tbody.innerHTML += `
                    <tr>
                        <td>${emp.employeeId}</td>
                        <td style="font-weight: bold;">${emp.fullName}</td>
                        <td>${emp.position || '—'}</td>
                        <td>${emp.contactPhone || '—'}</td>
                        <td style="color: #3b7a57;">${emp.login}</td>
                    </tr>
                `;
            });
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="5" class="error-message">Не вдалося завантажити дані.</td></tr>';
        }
    }

    const createEmpForm = document.getElementById('create-employee-form');
    if (createEmpForm) {
        const empError = document.getElementById('emp-error');
        const empSuccess = document.getElementById('emp-success');
        const btnCreateEmp = document.getElementById('btn-create-emp');

        createEmpForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            empError.style.display = 'none';
            empSuccess.style.display = 'none';

            const name = document.getElementById('emp-name').value.trim();
            const position = document.getElementById('emp-position').value;
            const phone = document.getElementById('emp-phone').value.trim();
            const login = document.getElementById('emp-login').value.trim();
            const password = document.getElementById('emp-password').value.trim();

            if (!name || !position || !phone || !login || !password) return;

            btnCreateEmp.disabled = true;
            btnCreateEmp.textContent = 'Збереження...';

            try {
                const response = await fetch('/api/Employee', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ FullName: name, Position: position, ContactPhone: phone, Login: login, Password: password })
                });

                if (response.ok) {
                    createEmpForm.reset();
                    empSuccess.style.display = 'block';
                    loadEmployees();
                    setTimeout(() => { empSuccess.style.display = 'none'; }, 4000);
                } else {
                    const errText = await response.text();
                    try {
                        const errData = JSON.parse(errText);
                        empError.textContent = errData.message || errData.errors[Object.keys(errData.errors)[0]][0];
                    } catch { empError.textContent = errText || 'Помилка сервера.'; }
                    empError.style.display = 'block';
                }
            } catch (error) {
                empError.textContent = "Немає зв'язку з сервером.";
                empError.style.display = 'block';
            } finally {
                btnCreateEmp.disabled = false;
                btnCreateEmp.textContent = 'Створити обліковий запис';
            }
        });
    }

    // =======================================================================
    // === 4. ВКЛАДКА 2: ТРАНЗАКЦІЇ ТА КВИТАНЦІЇ =============================
    // =======================================================================
    async function loadTransactions() {
        const tbody = document.getElementById('transactions-table-body');
        try {
            const response = await fetch('/api/Finance');
            if (!response.ok) throw new Error();
            const transactions = await response.json();
            tbody.innerHTML = '';

            if (transactions.length === 0) {
                tbody.innerHTML = '<tr><td colspan="7">Жодної транзакції ще не проведено.</td></tr>';
                return;
            }

            transactions.sort((a, b) => b.transactionId - a.transactionId);
            transactions.forEach(t => {
                const isIncome = (t.type && t.type.toLowerCase() === 'надходження');
                const typeColor = isIncome ? '#3b7a57' : '#d34c3d';
                const sign = isIncome ? '+' : '-';
                tbody.innerHTML += `
                    <tr>
                        <td>${t.transactionId}</td>
                        <td>${t.projectId ? `Проєкт №${t.projectId}` : '—'}</td>
                        <td>${t.contractId ? `Договір №${t.contractId}` : '—'}</td>
                        <td style="color: ${typeColor}; font-weight: bold;">${t.type}</td>
                        <td style="color: ${typeColor}; font-weight: bold;">${sign}${t.amount.toLocaleString('uk-UA')}</td>
                        <td>${t.purpose || '—'} ${t.employeeId ? `<br><small style="color:#555;">(Пр. №${t.employeeId})</small>` : ''}</td>
                        <td>${t.transactionDate}</td>
                    </tr>
                `;
            });
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="7" class="error-message">Не вдалося завантажити транзакції.</td></tr>';
        }
    }

    const transForm = document.getElementById('create-transaction-form');
    if (transForm) {
        const transError = document.getElementById('trans-error');
        const btnTransSubmit = document.getElementById('btn-create-trans');
        const receiptBlock = document.getElementById('receipt-block');

        transForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            transError.style.display = 'none';

            const projectIdStr = document.getElementById('trans-project-id').value.trim();
            const contractIdStr = document.getElementById('trans-contract-id').value.trim();
            const amount = document.getElementById('trans-amount').value;
            const purpose = document.getElementById('trans-purpose').value.trim();
            const targetEmpIdStr = document.getElementById('trans-employee-id').value.trim();
            const todayDate = new Date().toISOString().split('T')[0];

            if (!amount || !purpose) return;

            btnTransSubmit.disabled = true;
            btnTransSubmit.textContent = 'Обробка...';

            try {
                const payload = {
                    Type: "витрата",
                    Amount: parseFloat(amount),
                    Purpose: purpose,
                    TransactionDate: todayDate
                };

                if (projectIdStr) payload.ProjectId = parseInt(projectIdStr);
                if (contractIdStr) payload.ContractId = parseInt(contractIdStr);
                if (targetEmpIdStr) payload.EmployeeId = parseInt(targetEmpIdStr);

                const response = await fetch('/api/Finance', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    document.getElementById('receipt-date').textContent = todayDate;
                    document.getElementById('receipt-purpose').textContent = purpose;
                    document.getElementById('receipt-amount').textContent = parseFloat(amount).toLocaleString('uk-UA');

                    document.getElementById('receipt-project').textContent = projectIdStr || '';
                    document.getElementById('receipt-project-row').style.display = projectIdStr ? 'block' : 'none';

                    document.getElementById('receipt-contract').textContent = contractIdStr || '';
                    document.getElementById('receipt-contract-row').style.display = contractIdStr ? 'block' : 'none';

                    document.getElementById('receipt-emp').textContent = targetEmpIdStr || '';
                    document.getElementById('receipt-emp-row').style.display = targetEmpIdStr ? 'block' : 'none';

                    transForm.style.display = 'none';
                    receiptBlock.style.display = 'block';
                    loadTransactions();
                } else {
                    const errText = await response.text();
                    try {
                        const errData = JSON.parse(errText);
                        transError.textContent = errData.message || errData.errors[Object.keys(errData.errors)[0]][0];
                    } catch { transError.textContent = errText || 'Помилка сервера.'; }
                    transError.style.display = 'block';
                }
            } catch (error) {
                transError.textContent = "Немає зв'язку з сервером.";
                transError.style.display = 'block';
            } finally {
                btnTransSubmit.disabled = false;
                btnTransSubmit.textContent = 'Провести платіж';
            }
        });

        document.getElementById('btn-close-receipt').addEventListener('click', () => {
            receiptBlock.style.display = 'none';
            transForm.reset();
            transForm.style.display = 'grid';
        });
    }

    // =======================================================================
    // === 5. ВКЛАДКА 3: ПОСТАЧАЛЬНИКИ ТА ДОГОВОРИ ===========================
    // =======================================================================
    async function loadSuppliers() {
        const tbody = document.getElementById('suppliers-table-body');
        try {
            const response = await fetch('/api/Supplier');
            if (!response.ok) throw new Error();
            const suppliers = await response.json();
            tbody.innerHTML = '';
            if (suppliers.length === 0) {
                tbody.innerHTML = '<tr><td colspan="4">Немає зареєстрованих постачальників.</td></tr>';
                return;
            }
            suppliers.forEach(s => {
                tbody.innerHTML += `
                    <tr>
                        <td>${s.supplierId}</td>
                        <td style="font-weight: bold;">${s.companyName}</td>
                        <td>${s.edrpouCode}</td>
                        <td>${s.contacts || '—'}</td>
                    </tr>
                `;
            });
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="4" class="error-message">Не вдалося завантажити постачальників.</td></tr>';
        }
    }

    const supForm = document.getElementById('create-supplier-form');
    if (supForm) {
        const supError = document.getElementById('sup-error');
        const supSuccess = document.getElementById('sup-success');
        const btnSupSubmit = document.getElementById('btn-create-sup');

        supForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            supError.style.display = 'none';
            supSuccess.style.display = 'none';

            const name = document.getElementById('sup-name').value.trim();
            const edrpou = document.getElementById('sup-edrpou').value.trim();
            const contacts = document.getElementById('sup-contacts').value.trim();

            if (!name || !edrpou) {
                supError.textContent = 'Будь ласка, заповніть Назву компанії та код ЄДРПОУ.';
                supError.style.display = 'block';
                return;
            }

            btnSupSubmit.disabled = true;
            btnSupSubmit.textContent = 'Обробка...';

            try {
                const response = await fetch('/api/Supplier', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ CompanyName: name, EdrpouCode: edrpou, Contacts: contacts })
                });

                if (response.ok) {
                    supForm.reset();
                    supSuccess.style.display = 'block';
                    loadSuppliers();
                    setTimeout(() => { supSuccess.style.display = 'none'; }, 4000);
                } else {
                    const errText = await response.text();
                    try {
                        const errData = JSON.parse(errText);
                        if (errData.errors) {
                            const firstKey = Object.keys(errData.errors)[0];
                            supError.textContent = errData.errors[firstKey][0];
                        } else if (errData.message) {
                            supError.textContent = errData.message;
                        } else {
                            supError.textContent = 'Помилка валідації даних.';
                        }
                    } catch {
                        supError.textContent = errText || 'Помилка сервера.';
                    }
                    supError.style.display = 'block';
                }
            } catch {
                supError.textContent = "Помилка зв'язку з сервером.";
                supError.style.display = 'block';
            } finally {
                btnSupSubmit.disabled = false;
                btnSupSubmit.textContent = 'Додати постачальника';
            }
        });
    }

    async function loadContracts() {
        const tbody = document.getElementById('contracts-table-body');
        try {
            const response = await fetch('/api/Contract');
            if (!response.ok) throw new Error();
            const contracts = await response.json();
            tbody.innerHTML = '';
            if (contracts.length === 0) {
                tbody.innerHTML = '<tr><td colspan="7">Жодного договору ще не зареєстровано.</td></tr>';
                return;
            }
            contracts.sort((a, b) => b.contractId - a.contractId);
            contracts.forEach(c => {
                let statusHtml = `<span style="color: #555; font-weight: bold;">${c.status}</span>`;
                if (c.status === 'чинний') {
                    statusHtml = `
                        <span style="color: #3b7a57;">${c.status}</span>
                        <button onclick="closeContract(${c.contractId})" class="btn-simple" style="padding: 2px 8px; font-size: 0.8rem; border-color: #276345; margin-left: 10px; cursor: pointer;"> Завершити</button>
                    `;
                }

                tbody.innerHTML += `
                    <tr>
                        <td>${c.contractId}</td>
                        <td style="color: #276345; font-weight: bold;">Пост. №${c.supplierId}</td>
                        <td>Проєкт №${c.projectId}</td>
                        <td>${c.signingDate}</td>
                        <td>${c.subject}</td>
                        <td style="font-weight: bold;">${c.amount.toLocaleString('uk-UA')}</td>
                        <td>${statusHtml}</td>
                    </tr>
                `;
            });
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="7" class="error-message">Не вдалося завантажити договори.</td></tr>';
        }
    }

    window.closeContract = async function (contractId) {
        if (!confirm('Ви впевнені, що хочете перевести цей договір у статус "виконано"?')) return;

        try {
            const response = await fetch(`/api/Contract/${contractId}/status`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Status: "виконано" })
            });

            if (response.ok) {
                loadContracts();
            } else {
                alert('Не вдалося оновити статус договору на сервері.');
            }
        } catch (error) {
            alert('Помилка зв\'язку з сервером.');
        }
    };

    const conDateInput = document.getElementById('con-date');
    if (conDateInput) {
        const todayStr = new Date().toISOString().split('T')[0];
        conDateInput.value = todayStr;
        conDateInput.max = todayStr;
        conDateInput.min = todayStr;
        conDateInput.readOnly = true;
        conDateInput.style.backgroundColor = '#e5dcc3';
        conDateInput.style.color = '#555';
        conDateInput.style.cursor = 'not-allowed';
    }

    const conForm = document.getElementById('create-contract-form');
    if (conForm) {
        const conError = document.getElementById('con-error');
        const conSuccess = document.getElementById('con-success');
        const btnConSubmit = document.getElementById('btn-create-con');

        conForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            conError.style.display = 'none';
            conSuccess.style.display = 'none';

            const supId = document.getElementById('con-supplier-id').value;
            const projId = document.getElementById('con-project-id').value;
            const date = document.getElementById('con-date').value;
            const amount = document.getElementById('con-amount').value;
            const subject = document.getElementById('con-subject').value.trim();
            const quantity = document.getElementById('con-quantity').value;

            if (!supId || !projId || !date || !amount || !subject || !quantity) return;

            btnConSubmit.disabled = true;
            btnConSubmit.textContent = 'Обробка...';

            try {
                const response = await fetch('/api/Contract', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        SupplierId: parseInt(supId),
                        ProjectId: parseInt(projId),
                        SigningDate: date,
                        Amount: parseFloat(amount),
                        Subject: subject,
                        Quantity: parseInt(quantity)
                    })
                });

                if (response.ok) {
                    conForm.reset();
                    const todayStr = new Date().toISOString().split('T')[0];
                    conDateInput.value = todayStr;
                    conSuccess.style.display = 'block';
                    loadContracts();
                    setTimeout(() => { conSuccess.style.display = 'none'; }, 4000);
                } else {
                    const errText = await response.text();
                    try {
                        const errData = JSON.parse(errText);
                        if (errData.errors) {
                            const firstKey = Object.keys(errData.errors)[0];
                            conError.textContent = errData.errors[firstKey][0];
                        } else if (errData.message) {
                            conError.textContent = errData.message;
                        } else {
                            conError.textContent = 'Помилка валідації даних.';
                        }
                    } catch {
                        conError.textContent = errText || 'Помилка створення договору.';
                    }
                    conError.style.display = 'block';
                }
            } catch {
                conError.textContent = "Помилка зв'язку з сервером.";
                conError.style.display = 'block';
            } finally {
                btnConSubmit.disabled = false;
                btnConSubmit.textContent = 'Створити договір';
            }
        });
    }

    // =======================================================================
    // === 6. НОВА ВКЛАДКА: ПРОЄКТИ ТА ЗАЯВКИ (МЕНЕДЖЕР) =====================
    // =======================================================================
    async function loadProjects() {
        const container = document.getElementById('requests-container');
        try {
            const response = await fetch('/api/HelpRequest');
            if (!response.ok) throw new Error();
            const requests = await response.json();

            const newRequests = requests.filter(r => r.status && r.status.toLowerCase() === 'нове');

            container.innerHTML = '';
            if (newRequests.length === 0) {
                container.innerHTML = '<div style="padding: 20px; text-align: center; border: 2px dashed #3b2a1a;">Нових заявок немає. Можна пити каву! ☕</div>';
                return;
            }

            for (const r of newRequests) {
                let beneficiaryName = `Бенефіціар №${r.beneficiaryId}`;
                let beneficiaryPhone = "Завантаження...";

                try {
                    const benResponse = await fetch(`/api/Beneficiary/${r.beneficiaryId}`);
                    if (benResponse.ok) {
                        const benData = await benResponse.json();
                        beneficiaryName = benData.fullName || benData.name || beneficiaryName;
                        beneficiaryPhone = benData.contactPhone || benData.phone || benData.phoneNumber || "Не вказано";
                    } else {
                        beneficiaryPhone = "Дані недоступні";
                    }
                } catch {
                    beneficiaryPhone = "Помилка зв'язку";
                }

                const regDate = r.registrationDate ? new Date(r.registrationDate).toLocaleDateString('uk-UA') : 'Невідомо';
                const safeDesc = (r.needDescription || '').replace(/'/g, "\\'").replace(/"/g, "&quot;").replace(/\n/g, " ");

                const documentLinkHtml = r.documentPath
                    ? `<button type="button" onclick="alert('Імітація: Менеджер успішно переглянув та перевірив документи.');" style="color: #0056b3; font-weight: bold; border: 1px solid #b3a886; background-color: #e5dcc3; padding: 5px 10px; border-radius: 5px; cursor: pointer; font-size: 0.9rem;"> Перевірити прикріплені документи</button>`
                    : '<span style="color: #888;">Фото не додано</span>';

                container.innerHTML += `
                    <div style="border: 2px solid #3b2a1a; border-radius: 5px; background-color: #fff; padding: 20px; position: relative; margin-bottom: 15px;">
                        <div style="display: flex; justify-content: space-between; border-bottom: 2px dashed #e5dcc3; padding-bottom: 10px; margin-bottom: 15px;">
                            <span style="font-size: 1.2rem; font-weight: bold; color: #d34c3d;">Заявка №${r.requestId}</span>
                            <span style="color: #555;"> Дата реєстрації: ${regDate}</span>
                        </div>
                        
                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px; font-size: 1.05rem;">
                            <div><strong> Заявник:</strong> ${beneficiaryName}</div>
                            <div><strong> Телефон:</strong> <a href="tel:${beneficiaryPhone}" style="color: #276345; font-weight: bold; text-decoration: none;">${beneficiaryPhone}</a></div>
                            <div style="grid-column: span 2;"><strong> Адреса доставки:</strong> ${r.deliveryAddress || '<span style="color: #888;">Не вказана</span>'}</div>
                            <div style="grid-column: span 2; margin-top: 10px;"><strong> Підтверджуючі документи:</strong> <br><br> ${documentLinkHtml}</div>
                        </div>

                        <div style="background-color: #e5dcc3; padding: 15px; border-radius: 5px; margin-bottom: 20px; color: #3b2a1a;">
                            <strong style="display: block; margin-bottom: 5px;"> Опис проблеми та потреби:</strong>
                            <div style="font-style: italic; white-space: pre-wrap;">${r.needDescription || 'Опис відсутній...'}</div>
                        </div>

                        <div style="display: flex; justify-content: flex-end; gap: 10px;">
                            <button onclick="startProjectCreation(${r.requestId}, ${r.beneficiaryId}, '${safeDesc}')" class="btn-simple" style="background-color: #276345; color: #fbf3db; border: none; font-size: 1rem; padding: 10px 20px;"> Взяти в роботу (Створити проєкт)</button>
                            <button onclick="rejectHelpRequest(${r.requestId})" class="btn-simple" style="background-color: #fbf3db; color: #d34c3d; border-color: #d34c3d; font-size: 1rem; padding: 10px 20px;"> Відхилити</button>
                        </div>
                    </div>
                `;
            }
        } catch (error) {
            container.innerHTML = '<div class="error-message">Не вдалося завантажити заявки з бази. Перевірте з\'єднання.</div>';
        }
    }

    window.rejectHelpRequest = async function (requestId) {
        if (!confirm('Ви впевнені, що хочете відхилити цю заявку?')) return;
        try {
            const employeeId = parseInt(localStorage.getItem('employeeId'));
            const response = await fetch(`/api/HelpRequest/${requestId}/status`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Status: "відхилено", EmployeeId: employeeId })
            });

            if (response.ok) loadProjects();
            else alert('Не вдалося оновити статус заявки.');
        } catch (error) { alert('Помилка зв\'язку з сервером.'); }
    };

    let currentProcessingRequestId = null;

    window.startProjectCreation = function (requestId, beneficiaryId, description) {
        currentProcessingRequestId = requestId;
        document.getElementById('proj-beneficiary-id').value = beneficiaryId;
        document.getElementById('proj-description').value = `[Оригінальний запит бенефіціара №${requestId}]:\n${description}\n\n---\n(Тут ви можете відредагувати цей текст для публікації збору на сайті...)`;

        const formTitle = document.querySelector('#tab-projects .admin-card h3');
        if (formTitle) {
            formTitle.innerHTML = ` Створення проєкту (Обробка заявки №${requestId})`;
            formTitle.style.color = '#d34c3d';
        }

        document.getElementById('tab-projects').scrollIntoView({ behavior: 'smooth', block: 'start' });
        setTimeout(() => { document.getElementById('proj-title').focus(); }, 600);
    };

    const createProjForm = document.getElementById('create-project-form');
    if (createProjForm) {
        const projError = document.getElementById('proj-error');
        const projSuccess = document.getElementById('proj-success');
        const btnCreateProj = document.getElementById('btn-create-proj');

        createProjForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            projError.style.display = 'none';
            projSuccess.style.display = 'none';

            const title = document.getElementById('proj-title').value.trim();
            const budget = document.getElementById('proj-budget').value;
            const description = document.getElementById('proj-description').value.trim();
            const beneficiaryIdStr = document.getElementById('proj-beneficiary-id').value.trim();

            if (!title || !budget || !description) return;

            btnCreateProj.disabled = true;
            btnCreateProj.textContent = 'Обробка...';

            try {
                const currentEmployeeId = parseInt(localStorage.getItem('employeeId'));
                const payload = {
                    Title: title,
                    ApprovedBudget: parseFloat(budget),
                    Description: description,
                    Status: "активний",
                    StartDate: new Date().toISOString().split('T')[0],
                    EndDate: null,
                    EmployeeId: currentEmployeeId
                };

                if (beneficiaryIdStr) payload.BeneficiaryId = parseInt(beneficiaryIdStr);
                if (currentProcessingRequestId !== null) payload.HelpRequestId = parseInt(currentProcessingRequestId);

                const response = await fetch('/api/Project', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    currentProcessingRequestId = null;
                    const formTitle = document.querySelector('#tab-projects .admin-card h3');
                    if (formTitle) {
                        formTitle.innerHTML = `Створення проєкту`;
                        formTitle.style.color = '#3b7a57';
                    }

                    createProjForm.reset();
                    projSuccess.style.display = 'block';
                    setTimeout(() => { loadProjects(); }, 500);
                    setTimeout(() => { projSuccess.style.display = 'none'; }, 4000);
                } else {
                    const errText = await response.text();
                    try {
                        const errData = JSON.parse(errText);
                        if (errData.errors) {
                            const firstKey = Object.keys(errData.errors)[0];
                            projError.textContent = errData.errors[firstKey][0];
                        } else if (errData.message) projError.textContent = errData.message;
                        else projError.textContent = 'Помилка створення проєкту.';
                    } catch { projError.textContent = errText || 'Помилка сервера.'; }
                    projError.style.display = 'block';
                }
            } catch {
                projError.textContent = "Помилка зв'язку з сервером.";
                projError.style.display = 'block';
            } finally {
                btnCreateProj.disabled = false;
                btnCreateProj.textContent = 'Створити проєкт';
            }
        });
    }
    // =======================================================================
    // === 7. НОВА ВКЛАДКА: ВОЛОНТЕРИ ТА ЗАХОДИ (КООРДИНАТОР) ================
    // =======================================================================

    async function loadVolunteerProjects() {
        const container = document.getElementById('volunteer-projects-container');
        try {
            const response = await fetch('/api/Project');
            if (!response.ok) throw new Error();
            const allProjects = await response.json();

            const collectedProjects = allProjects.filter(p => p.status && p.status.toLowerCase() === 'зібрано');

            let allContracts = [];
            try {
                const contractsResponse = await fetch('/api/Contract');
                if (contractsResponse.ok) {
                    allContracts = await contractsResponse.json();
                }
            } catch (e) {
                console.warn('Не вдалося завантажити договори.');
            }

            const readyProjects = collectedProjects.filter(p => {
                const pendingContracts = allContracts.filter(c => c.projectId === p.projectId && c.status.toLowerCase() === 'чинний');
                return pendingContracts.length === 0;
            });

            container.innerHTML = '';

            if (collectedProjects.length > 0 && readyProjects.length === 0) {
                container.innerHTML = '<div style="padding: 20px; text-align: center; border: 2px dashed #d34c3d; color: #d34c3d;">Є проєкти, на які зібрано кошти, але закупівлі ще не завершено. Дочекайтеся, поки бухгалтер закриє всі договори.</div>';
                return;
            }

            if (readyProjects.length === 0) {
                container.innerHTML = '<div style="padding: 20px; text-align: center; border: 2px dashed #3b2a1a;">Наразі немає проєктів, готових до створення заходу.</div>';
                return;
            }

            for (const p of readyProjects) {
                const safeTitle = (p.title || '').replace(/'/g, "\\'").replace(/"/g, "&quot;");
                const safeDesc = (p.description || '').replace(/'/g, "\\'").replace(/"/g, "&quot;").replace(/\n/g, " ");

                container.innerHTML += `
                    <div style="border: 2px solid #3b7a57; border-radius: 5px; background-color: #fff; padding: 20px; box-shadow: 2px 2px 0px #e5dcc3; margin-bottom: 15px;">
                        <div style="font-size: 1.3rem; font-weight: bold; color: #3b7a57; margin-bottom: 10px;">Проєкт №${p.projectId}: ${p.title}</div>
                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-bottom: 15px;">
                            <div><strong>Бюджет:</strong> ${p.approvedBudget.toLocaleString('uk-UA')} ₴</div>
                            <div><strong>Статус:</strong> <span style="color: #3b7a57; font-weight: bold;">Готовий до реалізації</span></div>
                        </div>
                        <div style="background-color: #fbf3db; padding: 15px; border-radius: 5px; margin-bottom: 15px;">
                            <strong>Опис проєкту:</strong>
                            <div style="margin-top: 5px; font-style: italic; white-space: pre-wrap;">${p.description}</div>
                        </div>
                        
                        <div style="display: flex; justify-content: flex-end;">
                            <button onclick="startEventCreation(${p.projectId}, '${safeTitle}', '${safeDesc}', ${p.beneficiaryId || null}, ${p.helpRequestId || null})" class="btn-simple" style="background-color: #3b7a57; color: #fbf3db; border: none; padding: 10px 20px; font-size: 1rem;">
                                  Створити волонтерський захід
                            </button>
                        </div>
                    </div>
                `;
            }
        } catch (error) {
            container.innerHTML = '<div class="error-message">Не вдалося завантажити готові проєкти. Перевірте з\'єднання.</div>';
        }
    }

    window.startEventCreation = async function (projectId, projectTitle, projectDesc, beneficiaryId, helpRequestId) {
        document.getElementById('event-creation-card').style.display = 'block';
        document.getElementById('volunteer-list-card').style.display = 'none';
        document.getElementById('create-event-form').reset(); 

        document.getElementById('event-project-id').value = projectId;
        document.getElementById('event-proj-title').textContent = `№${projectId} - ${projectTitle}`;
        document.getElementById('event-title').value = `Роботи по проєкту: ${projectTitle}`;

        let pulledAddress = "";
        let pulledPhone = "";

        if (helpRequestId) {
            try {
                const reqResponse = await fetch(`/api/HelpRequest/${helpRequestId}`);
                if (reqResponse.ok) {
                    const reqData = await reqResponse.json();
                    if (reqData.deliveryAddress) pulledAddress = reqData.deliveryAddress;
                }
            } catch (e) { console.warn("Не вдалося завантажити адресу з запиту"); }
        }

        if (beneficiaryId) {
            try {
                const benResponse = await fetch(`/api/Beneficiary/${beneficiaryId}`);
                if (benResponse.ok) {
                    const benData = await benResponse.json();
                    if (benData.contactPhone) pulledPhone = benData.contactPhone;
                    if (benData.phoneNumber) pulledPhone = benData.phoneNumber;
                    if (benData.phone) pulledPhone = benData.phone;
                }
            } catch (e) { console.warn("Не вдалося завантажити телефон бенефіціара"); }
        }

        // 1. Автоматично вставляємо адресу в інпут
        if (pulledAddress) {
            document.getElementById('event-location').value = pulledAddress;
        }

        // 2. Додаємо телефон до тексту опису проєкту 
        let modifiedDesc = projectDesc;
        if (pulledPhone) {
            modifiedDesc = `[Телефон бенефіціара: ${pulledPhone}]\n\n${projectDesc}`;
        }
        document.getElementById('event-proj-desc').textContent = modifiedDesc;

        document.getElementById('tab-volunteers').scrollIntoView({ behavior: 'smooth', block: 'start' });
        setTimeout(() => { document.getElementById('event-datetime').focus(); }, 600);
    };

    const btnCancelEvent = document.getElementById('btn-cancel-event');
    if (btnCancelEvent) {
        btnCancelEvent.addEventListener('click', () => {
            document.getElementById('create-event-form').reset();
            document.getElementById('event-creation-card').style.display = 'none';
            document.getElementById('volunteer-list-card').style.display = 'block';
        });
    }

    const createEventForm = document.getElementById('create-event-form');
    if (createEventForm) {
        const eventError = document.getElementById('event-error');
        const eventSuccess = document.getElementById('event-success');
        const btnCreateEvent = document.getElementById('btn-create-event');

        createEventForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            eventError.style.display = 'none';
            eventSuccess.style.display = 'none';

            const projectId = document.getElementById('event-project-id').value;
            const title = document.getElementById('event-title').value.trim();
            const datetime = document.getElementById('event-datetime').value;
            const location = document.getElementById('event-location').value.trim();
            const spots = document.getElementById('event-spots').value;
            const contactPhone = document.getElementById('event-contact-phone').value.trim();
            const rawDescription = document.getElementById('event-description').value.trim();

            if (!projectId || !title || !datetime || !location || !rawDescription || !spots) return;

            // === 3. ПЕРЕВІРКА НА МИНУЛУ ДАТУ ===
            const selectedDate = new Date(datetime);
            const currentDate = new Date();

            if (selectedDate <= currentDate) {
                eventError.textContent = 'Помилка: Дата та час заходу не можуть бути в минулому!';
                eventError.style.display = 'block';
                return; 
            }

            const finalDescription = `[Кількість місць: ${spots}]\n[Контактний телефон організатора: ${contactPhone}]\n\n${rawDescription}`;

            btnCreateEvent.disabled = true;
            btnCreateEvent.textContent = 'Публікація...';

            try {
                const currentEmployeeId = parseInt(localStorage.getItem('employeeId'));

                const payload = {
                    ProjectId: parseInt(projectId),
                    EmployeeId: currentEmployeeId,
                    Title: title,
                    EventDate: datetime,
                    Location: location,
                    Description: finalDescription
                };

                const response = await fetch('/api/Event', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    createEventForm.reset();
                    eventSuccess.style.display = 'block';

                    setTimeout(() => {
                        eventSuccess.style.display = 'none';
                        document.getElementById('event-creation-card').style.display = 'none';
                        document.getElementById('volunteer-list-card').style.display = 'block';
                        loadVolunteerProjects();
                    }, 3000);
                } else {
                    const errText = await response.text();
                    eventError.textContent = errText || 'Помилка створення заходу.';
                    eventError.style.display = 'block';
                }
            } catch {
                eventError.textContent = "Помилка зв'язку з сервером.";
                eventError.style.display = 'block';
            } finally {
                btnCreateEvent.disabled = false;
                btnCreateEvent.textContent = 'Опублікувати захід';
            }
        });
    }
    // =======================================================================
    // === 8. НОВА ВКЛАДКА: ОБЛІК ГОДИН ВОЛОНТЕРІВ ===========================
    // =======================================================================

    async function loadActiveEvents() {
        const container = document.getElementById('active-events-container');
        try {
            // Отримуємо всі заходи
            const response = await fetch('/api/Event');
            if (!response.ok) throw new Error();
            const allEvents = await response.json();

            // Відфільтровуємо лише ті заходи, які прив'язані до проєктів зі статусом "захід"
            const activeEvents = allEvents.filter(e => e.projectStatus && e.projectStatus.toLowerCase() === 'захід');

            container.innerHTML = '';
            if (activeEvents.length === 0) {
                container.innerHTML = '<div style="padding: 20px; text-align: center; border: 2px dashed #3b2a1a;">Наразі немає активних заходів для обліку.</div>';
                return;
            }

            activeEvents.forEach(ev => {
                const safeTitle = (ev.title || '').replace(/'/g, "\\'").replace(/"/g, "&quot;");
                const evDate = new Date(ev.eventDate).toLocaleString('uk-UA', {
                    year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'
                });

                container.innerHTML += `
                    <div style="border: 2px solid #3b2a1a; border-radius: 5px; background-color: #fff; padding: 20px; display: flex; justify-content: space-between; align-items: center; box-shadow: 2px 2px 0px #e5dcc3;">
                        <div>
                            <div style="font-size: 1.2rem; font-weight: bold; color: #d34c3d; margin-bottom: 5px;">${ev.title}</div>
                            <div style="color: #555; font-size: 0.9rem;"> ${evDate} | ${ev.location}</div>
                        </div>
                        <button onclick="openAttendancePanel(${ev.eventId}, '${safeTitle}', '${evDate}')" class="btn-simple" style="background-color: #d34c3d; color: #fbf3db; border: none; padding: 8px 15px;">
                             Відмітити волонтерів
                        </button>
                    </div>
                `;
            });
        } catch (error) {
            container.innerHTML = '<div class="error-message">Не вдалося завантажити активні заходи. Перевірте з\'єднання.</div>';
        }
    }

    const btnBackToEvents = document.getElementById('btn-back-to-events');
    if (btnBackToEvents) {
        btnBackToEvents.addEventListener('click', () => {
            document.getElementById('event-attendance-card').style.display = 'none';
            document.getElementById('active-events-list-card').style.display = 'block';
        });
    }

    window.openAttendancePanel = async function (eventId, eventTitle, eventDate) {
        document.getElementById('active-events-list-card').style.display = 'none';
        document.getElementById('event-attendance-card').style.display = 'block';

        document.getElementById('attendance-event-title').textContent = eventTitle;
        document.getElementById('attendance-event-date').textContent = `📅 ${eventDate}`;

        const tbody = document.getElementById('attendance-table-body');
        tbody.innerHTML = '<tr><td colspan="4" style="text-align: center;">Завантаження списку волонтерів...</td></tr>';

        try {
            const response = await fetch(`/api/VolunteerParticipation/event/${eventId}`);
            if (!response.ok) {
                if (response.status === 404) {
                    tbody.innerHTML = '<tr><td colspan="4" style="text-align: center;">На цей захід ще не записався жоден волонтер.</td></tr>';
                } else {
                    throw new Error();
                }
                return;
            }

            const participations = await response.json();
            tbody.innerHTML = '';

            participations.forEach(p => {
                const isRegistered = p.statusPresence === 'зареєстровано' ? 'selected' : '';
                const isPresent = p.statusPresence === 'присутній' ? 'selected' : '';
                const isAbsent = p.statusPresence === 'відсутній' ? 'selected' : '';

                const hours = p.hoursWorked || 0;

                tbody.innerHTML += `
                    <tr>
                        <td style="font-weight: bold; font-size: 1.1rem;">№ ${p.volunteerId}</td>
                        <td>
                            <select id="status-${p.participationId}" class="retro-input" style="padding: 5px; width: 100%;">
                                <option value="зареєстровано" ${isRegistered}>Зареєстровано</option>
                                <option value="присутній" ${isPresent}>Присутній</option>
                                <option value="відсутній" ${isAbsent}>Відсутній</option>
                            </select>
                        </td>
                        <td>
                            <input type="number" id="hours-${p.participationId}" class="retro-input" value="${hours}" min="0" step="0.5" style="padding: 5px; width: 80px;">
                        </td>
                        <td>
                            <button onclick="saveAttendance(${p.participationId})" class="btn-simple" style="padding: 4px 10px; font-size: 0.9rem; background-color: #3b7a57; color: #fbf3db; border: none;"> Зберегти</button>
                        </td>
                    </tr>
                `;
            });
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="4" class="error-message">Не вдалося завантажити список волонтерів.</td></tr>';
        }
    };

    window.saveAttendance = async function (participationId) {
        const newStatus = document.getElementById(`status-${participationId}`).value;
        const newHours = document.getElementById(`hours-${participationId}`).value;

        try {
            const response = await fetch(`/api/VolunteerParticipation/${participationId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    StatusPresence: newStatus,
                    HoursWorked: parseFloat(newHours)
                })
            });

            if (response.ok) {
                alert('Дані успішно збережено!');
            } else {
                alert('Помилка при збереженні даних на сервері.');
            }
        } catch (error) {
            alert('Помилка зв\'язку з сервером.');
        }
    };
});