const API_URL = '/api/Project';
const FINANCE_API = '/api/Finance'; 

document.addEventListener('DOMContentLoaded', () => {
    loadProjects();
    checkAuth();
});

function checkAuth() {
    const authLink = document.getElementById('auth-link');
    const userName = localStorage.getItem('userName');

    if (userName) {
        authLink.textContent = `профіль (${userName})`;
        authLink.href = 'profile.html';

        const logoutBtn = document.createElement('a');
        logoutBtn.href = '#';
        logoutBtn.textContent = 'вийти';
        logoutBtn.style.marginLeft = '15px';
        logoutBtn.style.color = '#d34c3d';

        logoutBtn.onclick = (e) => {
            e.preventDefault();
            localStorage.clear();
            window.location.reload();
        };

        authLink.parentNode.appendChild(logoutBtn);
    }
}

async function loadProjects() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error('Помилка');
        const projects = await response.json();

        let allTransactions = [];
        try {
            const financeRes = await fetch(FINANCE_API);
            if (financeRes.ok) {
                allTransactions = await financeRes.json();
            }
        } catch (e) {
            console.log("Контролер транзакцій недоступний");
        }

        renderProjects(projects, allTransactions);
    } catch (error) {
        document.getElementById('projects-container').innerHTML =
            '<p class="loading-text">Не вдалося завантажити дані :(</p>';
    }
}

function renderProjects(projects, transactions) {
    const container = document.getElementById('projects-container');
    container.innerHTML = '';

    // === Фільтруємо проєкти за статусом ===
    const allowedStatuses = ['активний', 'зібрано', 'захід'];

    const visibleProjects = projects.filter(project => {
        const status = (project.status || '').toLowerCase().trim();
        return allowedStatuses.includes(status);
    });

    if (visibleProjects.length === 0) {
        container.innerHTML = '<p class="loading-text">Наразі немає активних зборів.</p>';
        return;
    }

    visibleProjects.forEach(project => {

        const projId = project.projectId || project.id || project.ProjectId;

        let collectedAmount = 0;
        if (transactions.length > 0) {
            const projTransactions = transactions.filter(t =>
                (t.projectId === projId || t.ProjectId === projId) &&
                ((t.type && t.type.toLowerCase() === "надходження") || (t.Type && t.Type.toLowerCase() === "надходження"))
            );
            collectedAmount = projTransactions.reduce((sum, t) => sum + (t.amount || t.Amount || 0), 0);
        }

        let progressPercent = 0;
        if (project.approvedBudget > 0) {
            progressPercent = (collectedAmount / project.approvedBudget) * 100;
            if (progressPercent > 100) progressPercent = 100;
        }

        let shortDescription = project.description;
        if (shortDescription && shortDescription.length > 100) {
            shortDescription = shortDescription.substring(0, 100) + "...";
        }

        // HTML картки (з виправленим перенесенням тексту)
        const card = `
            <div class="project-card">
                <h3>${project.title}</h3>
                
                <p class="project-desc" style="word-wrap: break-word; overflow-wrap: break-word; word-break: break-word;">
                    ${shortDescription}
                </p>
                
                <div class="status-text">Статус: ${project.status}</div>
                
                <div class="progress-bg">
                    <div class="progress-fill" style="width: ${progressPercent}%;"></div>
                </div>
                
                <div class="progress-stats">
                    <span>Зібрано: ${collectedAmount.toLocaleString('uk-UA')} грн</span>
                    <span>Ціль: ${project.approvedBudget.toLocaleString('uk-UA')} грн</span>
                </div>
                
                <button class="btn-simple" onclick="window.location.href='project.html?id=${projId}'">Підтримати</button>
            </div>
        `;
        container.innerHTML += card;
    });
}