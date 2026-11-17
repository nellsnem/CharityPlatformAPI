document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');

    // Обробка входу
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const errorDiv = document.getElementById('login-error');
        errorDiv.textContent = '';

        const phone = document.getElementById('login-phone').value;
        const password = document.getElementById('login-password').value;

        try {
            const response = await fetch('/api/User/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Phone: phone, Password: password })
            });

            const text = await response.text();
            let data = {};
            try { data = text ? JSON.parse(text) : {}; } catch (err) { }

            if (!response.ok) {
                throw new Error(data.message || 'Невірний логін або пароль');
            }

            localStorage.setItem('userId', data.userId);
            localStorage.setItem('userName', data.fullName);

            window.location.href = 'index.html';

        } catch (error) {
            errorDiv.textContent = error.message;
        }
    });

    // Обробка реєстрації
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const errorDiv = document.getElementById('reg-error');
        const successDiv = document.getElementById('reg-success');
        errorDiv.textContent = '';
        successDiv.textContent = '';

        const fullName = document.getElementById('reg-name').value;
        const phone = document.getElementById('reg-phone').value;
        const password = document.getElementById('reg-password').value;

        try {
            const response = await fetch('/api/User/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ FullName: fullName, Phone: phone, Password: password })
            });

            const text = await response.text();
            let data = {};
            try { data = text ? JSON.parse(text) : {}; } catch (err) { }

            if (!response.ok) {
                if (data.errors) {
                    const firstKey = Object.keys(data.errors)[0];
                    throw new Error(data.errors[firstKey][0]);
                }
                throw new Error(data.message || 'Помилка реєстрації. Перевірте правильність даних.');
            }

            successDiv.textContent = 'Успішно! Тепер ви можете увійти зліва.';
            registerForm.reset();

        } catch (error) {
            errorDiv.textContent = error.message;
        }
    });
});