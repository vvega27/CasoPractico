//  Config 
const base = "https://localhost:7281/";
const urlList = `${base}api/Tasks`;
const urlItem = (id) => `${base}api/Tasks/${id}`;


const tasks = Array.isArray(window.tasksData) ? window.tasksData.slice() : [];


window.toast = function (title, body, variant = 'info') {
    const zone = document.getElementById('toastZone');
    if (!zone) return console.log(title, body);
    const id = 't' + Date.now();
    const html = `
  <div id="${id}" class="toast align-items-center text-bg-${variant} border-0" role="status" aria-live="polite" aria-atomic="true">
    <div class="d-flex">
      <div class="toast-body">
        <strong>${title}</strong>${body ? ` — ${body}` : ''}
      </div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
    </div>
  </div>`;
    zone.insertAdjacentHTML('beforeend', html);
    const el = document.getElementById(id);
    const t = new bootstrap.Toast(el, { delay: 2200 });
    t.show();
    el.addEventListener('hidden.bs.toast', () => el.remove());
};


const norm = (s) => (s ?? "").trim();
const byId = (id) => document.getElementById(id);
const selCardsRow = () => document.getElementById("cardsRow");

function fmtDateISOFromDate(v) {
    if (!v) return new Date().toISOString();
    const dt = new Date(v + "T00:00:00");
    return new Date(dt.getTime() - dt.getTimezoneOffset() * 60000).toISOString();
}
function fmtIsoFromDTLocal(v) {
    if (!v) return new Date().toISOString();
    return new Date(v).toISOString();
}
function extractIdFromLocation(loc) {
    if (!loc) return undefined;
    const m = /\/(\d+)$/.exec(loc);
    return m ? Number(m[1]) : undefined;
}
function updateCount() {
    const el = document.querySelector(".fw-semibold");
    if (el) el.textContent = String(tasks.length);
}


async function apiPost(task) {
    const res = await fetch(urlList, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task)
    });
    if (!res.ok) throw new Error(`POST ${res.status}`);
    let data = null;
    try { data = await res.json(); } catch { /* no body */ }
    if (data && typeof data === "object") return data;
    const id = extractIdFromLocation(res.headers.get("Location"));
    return { ...task, id };
}
async function apiPut(id, task) {
    const res = await fetch(urlItem(id), {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task)
    });
    if (!res.ok) throw new Error(`PUT ${res.status}`);
    try { return await res.json(); } catch { return task; }
}
async function apiDelete(id) {
    const res = await fetch(urlItem(id), { method: "DELETE" });
    if (!res.ok) throw new Error(`DELETE ${res.status}`);
}

function taskCardHtml(t) {
    const statusText = norm(t.status);
    const statusUpper = statusText.toUpperCase();
    const isOpen = statusUpper === "OPEN" || statusUpper === "PENDING" || statusUpper === "TRUE";
    const approvedState = t.approved === true ? "true" : "null";

    const due = t.dueDate ? new Date(t.dueDate) : null;
    const created = t.createdAt ? new Date(t.createdAt) : null;
    const dueStr = due && !isNaN(due) ? `${due.getFullYear()}-${String(due.getMonth() + 1).padStart(2, '0')}-${String(due.getDate()).padStart(2, '0')}` : "—";
    const createdStr = created && !isNaN(created) ?
        `${created.getFullYear()}-${String(created.getMonth() + 1).padStart(2, '0')}-${String(created.getDate()).padStart(2, '0')} ${String(created.getHours()).padStart(2, '0')}:${String(created.getMinutes()).padStart(2, '0')}` : "—";

    return `
  <div id="Task-${t.id}" class="col-12 col-sm-6 col-lg-4">
    <div class="border border-1 bg-body-secondary rounded-2 border-light-subtle py-3 px-3 h-100">
      <div class="d-flex justify-content-between align-items-center">
        <span class="text-dark ps-1 fw-semibold">Task Id# ${t.id}</span>
        <div class="dropdown">
          <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">Actions</button>
          <ul class="dropdown-menu dropdown-menu-end">
            <li><a class="dropdown-item" data-id="${t.id}" id="edit-${t.id}" href="#">Edit</a></li>
            <li><a class="dropdown-item text-danger" href="#" data-id="${t.id}" id="delete-${t.id}">Delete</a></li>
          </ul>
        </div>
      </div>

      <div class="mt-2">
        ${isOpen
            ? `<span class="badge text-bg-success" id="status-open-${t.id}">${statusText}</span>`
            : `<span class="badge text-bg-danger" id="status-close-${t.id}">${statusText}</span>`
        }
        ${approvedState === "true"
            ? `<span class="badge text-bg-primary ms-2">Approved</span>`
            : `<span class="badge text-bg-secondary ms-2">Pending approval</span>`
        }
      </div>

      <div class="mt-2 d-flex gap-2 align-items-center flex-wrap" id="inline-edit-${t.id}">
        <select id="statusSel-${t.id}" class="form-select form-select-sm w-auto">
          <option value="Open" ${statusUpper === "OPEN" ? "selected" : ""}>Open</option>
          <option value="Pending" ${statusUpper === "PENDING" ? "selected" : ""}>Pending</option>
          <option value="Closed" ${statusUpper === "CLOSED" ? "selected" : ""}>Closed</option>
        </select>
        <select id="approvedSel-${t.id}" class="form-select form-select-sm w-auto">
          <option value="true" ${approvedState === "true" ? "selected" : ""}>Approved</option>
          <option value="null" ${approvedState === "null" ? "selected" : ""}>Pending</option>
        </select>
        <button class="btn btn-sm btn-outline-primary btn-update" data-id="${t.id}">Update</button>
      </div>

      <div class="mt-3">
        <p class="fw-bold mb-1" data-field="name" id="name-${t.id}">${t.name}</p>
        <p class="mb-2 text-muted" data-field="description" id="description-${t.id}">${norm(t.description) || "—"}</p>
      </div>

      <div class="row text-dark justify-content-between small">
        <div class="col text-start" id="dueDate-${t.id}">
          <strong>Due:</strong> ${dueStr}
        </div>
        <div class="col text-end" id="createdDate-${t.id}">
          <strong>Created:</strong> ${createdStr}
        </div>
      </div>
    </div>
  </div>`;
}

function appendTaskCard(t) {
    const row = selCardsRow();
    if (!row) return;
    row.insertAdjacentHTML("afterbegin", taskCardHtml(t));
}

function replaceTaskCard(t) {
    const node = byId(`Task-${t.id}`);
    if (!node) return appendTaskCard(t);
    node.outerHTML = taskCardHtml(t);
}

const addForm = byId("addTaskForm");
const btnAdd = byId("addTaskBtn");
const btnSave = byId("saveTaskBtn");
const btnCancel = byId("cancelTaskBtn");
const inputUser = byId("newUsername");
const inputName = byId("newName");
const inputDesc = byId("newDescription");
const inputDue = byId("newDueDate");
const inputAt = byId("newCreatedAt");

function showAddForm(show) {
    if (!addForm) return;
    addForm.classList.toggle("d-none", !show);
}
btnAdd?.addEventListener("click", (e) => { e.preventDefault(); showAddForm(true); });
btnCancel?.addEventListener("click", (e) => {
    e.preventDefault();
    inputName.value = ""; inputDesc.value = ""; inputDue.value = "";
    inputAt.value = new Date().toISOString().slice(0, 16); 
    inputUser.value = "";
    showAddForm(false);
});

btnSave?.addEventListener("click", async (e) => {
    e.preventDefault();
    const name = norm(inputName.value);
    const description = norm(inputDesc.value);
    if (!name) { toast("Error", "Name is required", "danger"); return; }

    const payload = {
        name,
        description,
        status: "Pending",                                  
        dueDate: fmtDateISOFromDate(inputDue.value),
        createdAt: fmtIsoFromDTLocal(inputAt.value),
        approved: null                                      
    };

    try {
        const created = await apiPost(payload);
        const newTask = {
            id: created.id ?? Math.max(0, ...tasks.map(t => t.id || 0)) + 1,
            name: created.name ?? payload.name,
            description: created.description ?? payload.description,
            status: created.status ?? payload.status,
            dueDate: created.dueDate ?? payload.dueDate,
            createdAt: created.createdAt ?? payload.createdAt,
            approved: created.approved ?? payload.approved
        };
        tasks.unshift(newTask);
        appendTaskCard(newTask);
        updateCount();

        inputName.value = ""; inputDesc.value = ""; inputDue.value = "";
        inputAt.value = new Date().toISOString().slice(0, 16);
        inputUser.value = "";
        showAddForm(false);
        toast("Listo", "Task creada", "success");
    } catch (err) {
        console.error(err);
        toast("Error", "No se pudo crear", "danger");
    }
});


document.addEventListener("click", (e) => {
    const btn = e.target.closest(".btn-update");
    if (!btn) return;

    e.preventDefault();
    const id = Number(btn.dataset.id);
    const t = tasks.find(x => x.id === id);
    if (!t) { toast("Error", "Task not found", "danger"); return; }

    const statusSel = byId(`statusSel-${id}`);
    const approvedSel = byId(`approvedSel-${id}`);
    if (!statusSel || !approvedSel) { toast("Error", "Controles faltantes", "danger"); return; }

    const status = statusSel.value;
    const apprVal = approvedSel.value;
    const approved = apprVal === "true" ? true : null; 

    const payload = {
        id: t.id,
        name: t.name,
        description: t.description,
        status,
        dueDate: t.dueDate,
        createdAt: t.createdAt,
        approved
    };

    apiPut(id, payload)
        .then(() => {
            Object.assign(t, payload);
            replaceTaskCard(t);
            toast("OK", "Task actualizada", "success");
        })
        .catch(err => {
            console.error(err);
            toast("Error", "No se pudo actualizar", "danger");
        });
});

document.addEventListener("click", (e) => {
    const a = e.target.closest("a");
    if (!a) return;

    // Delete
    if (a.id?.startsWith("delete-")) {
        e.preventDefault();
        const id = Number(a.dataset.id || a.id.split("-")[1]);
        if (!confirm(`Delete task #${id}?`)) return;

        apiDelete(id)
            .then(() => {
                const idx = tasks.findIndex(t => t.id === id);
                if (idx >= 0) tasks.splice(idx, 1);
                byId(`Task-${id}`)?.remove();
                updateCount();
                toast("OK", "Task borrada", "success");
            })
            .catch(err => {
                console.error(err);
                toast("Error", "No se pudo borrar", "danger");
            });
        return;
    }

    // Edit → modal
    if (a.id?.startsWith("edit-")) {
        e.preventDefault();
        const id = Number(a.dataset.id || a.id.split("-")[1]);
        const t = tasks.find(x => x.id === id);
        if (!t) { toast("Error", "Task not found", "danger"); return; }

        byId('editId').value = t.id;
        byId('editName').value = t.name ?? '';
        byId('editDescription').value = t.description ?? '';

        const d = new Date(t.dueDate);
        byId('editDueDate').value = (!isNaN(d)) ? `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}` : '';

        const c = new Date(t.createdAt);
        byId('editCreatedAt').value = (!isNaN(c)) ?
            `${c.getFullYear()}-${String(c.getMonth() + 1).padStart(2, '0')}-${String(c.getDate()).padStart(2, '0')}T${String(c.getHours()).padStart(2, '0')}:${String(c.getMinutes()).padStart(2, '0')}` : '';

        new bootstrap.Modal(byId('editTaskModal')).show();
        return;
    }
});

// Guardar
document.addEventListener('click', async (e) => {
    const btn = e.target.closest('#editSaveBtn');
    if (!btn) return; 

    try {
        const id = Number(document.getElementById('editId').value);
        const t = tasks.find(x => x.id === id);
        if (!t) { toast('Error', 'Task not found', 'danger'); return; }

        const name = (document.getElementById('editName').value || '').trim() || t.name;
        const description = (document.getElementById('editDescription').value || '').trim() || t.description;

        const dueRaw = document.getElementById('editDueDate').value;        
        const atRaw = document.getElementById('editCreatedAt').value;      

        const payload = {
            id: t.id,
            name,
            description,
            status: t.status,                                     
            dueDate: dueRaw ? fmtDateISOFromDate(dueRaw) : t.dueDate,
            createdAt: atRaw ? fmtIsoFromDTLocal(atRaw) : t.createdAt,
            approved: t.approved                                 
        };

        console.log('PUT payload', payload); 

        await apiPut(id, payload);            
        Object.assign(t, payload);
        replaceTaskCard(t);

        const modalEl = document.getElementById('editTaskModal');
        bootstrap.Modal.getInstance(modalEl)?.hide();

        toast('Listo', 'Cambios guardados', 'success');
    } catch (err) {
        console.error('Edit save error:', err);
        toast('Error', 'No se pudo guardar', 'danger');
    }
});

// init
updateCount();
