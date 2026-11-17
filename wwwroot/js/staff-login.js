document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('staff-login-form');
    const loginInput = document.getElementById('staff-login');
    const passwordInput = document.getElementById('staff-password');
    const errorDiv = document.getElementById('server-error');

    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            errorDiv.style.display = 'none';

            const login = loginInput.value.trim();
            const password = passwordInput.value.trim();

            if (!login || !password) {
                errorDiv.textContent = 'Будь ласка, введіть логін та пароль.';
                errorDiv.style.display = 'block';
                return;
            }

            try {
                const response = await fetch('/api/Employee/login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ Login: login, Password: password })
                });

                if (response.ok) {
                    const data = await response.json();

                    // Зберігаємо дані працівника
                    localStorage.setItem('employeeId', data.employeeId);
                    localStorage.setItem('employeeName', data.fullName);
                    localStorage.setItem('employeeRole', data.position);

                    // Переходимо в адмін-панель
                    window.location.href = 'staff-panel.html';
                } else {
                    const errText = await response.text();
                    try {
                        const errData = JSON.parse(errText);
                        errorDiv.textContent = errData.message || 'Невірний логін або пароль.';
                    } catch {
                        errorDiv.textContent = 'Помилка авторизації.';
                    }
                    errorDiv.style.display = 'block';
                }
            } catch (error) {
                errorDiv.textContent = 'Немає зв\'язку з сервером.';
                errorDiv.style.display = 'block';
            }
        });
    }
});