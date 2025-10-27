(function () {
    const base = "https://localhost:7281/";
    const urlAssign = (userId) => `${base}api/users/${userId}/role`;

    function toast(title, body, variant = 'info') {
        const zone = document.getElementById('toastZone');
        const id = 't' + Date.now();
        zone?.insertAdjacentHTML('beforeend', `
      <div id="${id}" class="toast align-items-center text-bg-${variant} border-0" role="status" aria-live="polite" aria-atomic="true">
        <div class="d-flex">
          <div class="toast-body"><strong>${title}</strong>${body ? ` — ${body}` : ''}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
      </div>`);
        const el = document.getElementById(id);
        if (!el) return;
        new bootstrap.Toast(el, { delay: 2000 }).show();
        el.addEventListener('hidden.bs.toast', () => el.remove());
    }

    async function assignRole(userId, roleId) {
        const res = await fetch(urlAssign(userId), {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ roleId: Number(roleId) })
        });
        if (!res.ok) {
            const msg = await res.text().catch(() => "Error");
            throw new Error(msg);
        }
    }

    document.addEventListener('click', async (e) => {
        const btn = e.target.closest('.saveRoleBtn');
        if (!btn) return;

        const tr = btn.closest('tr');
        const userId = Number(tr?.dataset.user);
        const sel = tr?.querySelector('.roleSel');
        const roleId = Number(sel?.value);

        if (!userId || !roleId) { toast("Error", "Usuario o rol inválido", "danger"); return; }

        btn.setAttribute('disabled', 'disabled');
        try {
            await assignRole(userId, roleId);
            toast("OK", "Rol actualizado", "success");
        } catch (err) {
            console.error(err);
            toast("Error", err.message || "No se pudo actualizar el rol", "danger");
        } finally {
            btn.removeAttribute('disabled');
        }
    });
})();
