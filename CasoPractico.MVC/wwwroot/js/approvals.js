(function () {
    const base = "https://localhost:7281/";
    const urlItem = (id) => `${base}api/Tasks/${id}`;

    const todos = Array.isArray(window.approvalsData) ? window.approvalsData.slice() : [];

    function toast(title, body, variant = 'info') {
        const zone = document.getElementById('toastZone');
        if (!zone) return console.log(title, body);
        const id = 't' + Date.now();
        zone.insertAdjacentHTML('beforeend', `
      <div id="${id}" class="toast align-items-center text-bg-${variant} border-0" role="status" aria-live="polite" aria-atomic="true">
        <div class="d-flex">
          <div class="toast-body"><strong>${title}</strong>${body ? ` — ${body}` : ''}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
      </div>`);
        const el = document.getElementById(id);
        const t = new bootstrap.Toast(el, { delay: 2200 }); t.show();
        el.addEventListener('hidden.bs.toast', () => el.remove());
    }

    async function apiPut(id, task) {
        const res = await fetch(`${base}api/Tasks/${id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(task)
        });
        if (!res.ok) {
            const msg = await res.text().catch(() => "Error en PUT");
            throw new Error(msg || `PUT ${res.status}`);
        }
        try { return await res.json(); } catch { return task; }
    }

    function findTask(id) { return todos.find(x => x.id === id); }

    async function setApproval(id, approved) {
        const t = findTask(id);
        if (!t) { toast("Error", "Task not found", "danger"); return; }

        const payload = {
            id: t.id,
            name: t.name,
            description: t.description,
            status: t.status,
            dueDate: t.dueDate,
            createdAt: t.createdAt,
            approved: approved
        };

        await apiPut(id, payload);
        Object.assign(t, payload);

        const card = document.getElementById(`card-${id}`);
        if (card) {
            const badge = card.querySelector(".badge");
            if (badge) {
                if (approved === true) { badge.textContent = "Approved"; badge.className = "badge text-bg-primary"; }
                else if (approved === false) { badge.textContent = "Denied"; badge.className = "badge text-bg-secondary"; }
                else { badge.textContent = "Pending"; badge.className = "badge text-bg-warning"; }
            }
        }
        toast("OK", "Estado actualizado", "success");
    }

    document.addEventListener("click", (e) => {
        const a = e.target.closest(".btn-approve");
        const d = e.target.closest(".btn-deny");
        if (a) {
            e.preventDefault();
            const id = Number(a.dataset.id);
            setApproval(id, true).catch(err => {
                console.error(err);
                toast("Error", err.message || "No se pudo aprobar", "danger");
            });
        }
        if (d) {
            e.preventDefault();
            const id = Number(d.dataset.id);
            setApproval(id, false).catch(err => {
                console.error(err);
                toast("Error", err.message || "No se pudo denegar", "danger");
            });
        }
    });
})();
