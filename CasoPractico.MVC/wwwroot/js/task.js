const base = "https://localhost:7281/";
const urlList = `${base}api/Tasks`;
const urlItem = (id) => `${base}api/Tasks/${id}`;


const tasks = Array.isArray(window.tasksData) ? window.tasksData.slice() : [];

const norm = (s) => (s ?? "").trim();
const isOpen = (status) => {
    const t = norm(status).toUpperCase();
    return t === "OPEN" || t === "PENDING" || t === "TRUE";
};
const fmtDate = (d) => {
    try {
        if (!d) return "—";
        const dt = typeof d === "string" ? new Date(d) : d;
        if (isNaN(dt)) return "—";
        return `${dt.getFullYear()}-${String(dt.getMonth() + 1).padStart(2, "0")}-${String(dt.getDate()).padStart(2, "0")}`;
    } catch { return "—"; }
};
const fmtDateTime = (d) => {
    try {
        if (!d) return "—";
        const dt = typeof d === "string" ? new Date(d) : d;
        if (isNaN(dt)) return "—";
        return `${fmtDate(dt)} ${String(dt.getHours()).padStart(2, "0")}:${String(dt.getMinutes()).padStart(2, "0")}`;
    } catch { return "—"; }
};
const updateCount = () => {
    const el = document.querySelector(".fw-semibold");
    if (el) el.textContent = String(tasks.length);
};
const toast = (msg) => console.log(msg); 

function taskCardHtml(t) {
    const statusText = norm(t.status);
    const open = isOpen(statusText);
    const approvedBadge = (t.approved === true)
        ? `<span class="badge text-bg-primary ms-2">Approved</span>`
        : (t.approved === false)
            ? `<span class="badge text-bg-secondary ms-2">Pending approval</span>`
            : "";

    return `
  <div id="Task-${t.id}" class="col-12 col-sm-6 col-lg-4">
    <div class="border border-1 bg-body-secondary rounded-2 border-light-subtle py-3 px-3 h-100">
      <div class="d-flex justify-content-between align-items-center">
        <span class="text-dark ps-1 fw-semibold">Task Id# ${t.id}</span>

        <div class="dropdown">
          <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
            Actions
          </button>
          <ul class="dropdown-menu dropdown-menu-end">
            <li><a class="dropdown-item" data-id="${t.id}" id="edit-${t.id}" href="#">Edit</a></li>
            <li><a class="dropdown-item text-danger" href="#" data-id="${t.id}" id="delete-${t.id}">Delete</a></li>
          </ul>
        </div>
      </div>

      <div class="mt-2">
        ${open
            ? `<span class="badge text-bg-success" id="status-open-${t.id}">${statusText}</span>`
            : `<span class="badge text-bg-danger" id="status-close-${t.id}">${statusText}</span>`
        }
        ${approvedBadge}
      </div>

      <div class="mt-3">
        <p class="fw-bold mb-1" data-field="name" id="name-${t.id}">${t.name}</p>
        <p class="mb-2 text-muted" data-field="description" id="description-${t.id}">${norm(t.description) || "—"}</p>
      </div>

      <div class="row text-dark justify-content-between small">
        <div class="col text-start" id="dueDate-${t.id}">
          <strong>Due:</strong> ${fmtDate(t.dueDate)}
        </div>
        <div class="col text-end" id="createdDate-${t.id}">
          <strong>Created:</strong> ${fmtDateTime(t.createdAt)}
        </div>
      </div>
    </div>
  </div>`;
}

function appendTaskCard(t) {
    const row = document.querySelector(".row.g-3.justify-content-center, .row.gap-2.justify-content-center");
    if (!row) return;
    row.insertAdjacentHTML("afterbegin", taskCardHtml(t));
}

function replaceTaskCard(t) {
    const node = document.getElementById(`Task-${t.id}`);
    if (!node) return appendTaskCard(t);
    node.outerHTML = taskCardHtml(t);
}

async function apiPost(task) {
    const res = await fetch(urlList, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task)
    });
    if (!res.ok) throw new Error(`POST failed: ${res.status}`);
    return await res.json();
}
async function apiPut(id, task) {
    const res = await fetch(urlItem(id), {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task)
    });
    if (!res.ok) throw new Error(`PUT failed: ${res.status}`);
    try { return await res.json(); } catch { return task; }
}
async function apiDelete(id) {
    const res = await fetch(urlItem(id), { method: "DELETE" });
    if (!res.ok) throw new Error(`DELETE failed: ${res.status}`);
}

// ==== Add form ====
const addForm = document.getElementById("addTaskForm");
const btnAdd = document.getElementById("addTaskBtn");
const btnSave = document.getElementById("saveTaskBtn");
const btnCancel = document.getElementById("cancelTaskBtn");
const inputName = document.getElementById("newName");
const inputDesc = document.getElementById("newDescription");

function showAddForm(show) {
    if (!addForm) return;
    if (show) addForm.classList.remove("d-none");
    else addForm.classList.add("d-none");
}

btnAdd?.addEventListener("click", (e) => {
    e.preventDefault();
    showAddForm(true);
});

btnCancel?.addEventListener("click", (e) => {
    e.preventDefault();
    inputName.value = "";
    inputDesc.value = "";
    if (inputReporter) inputReporter.value = "";
    showAddForm(false);
});

btnSave?.addEventListener("click", async (e) => {
    e.preventDefault();
    const name = norm(inputName.value);
    const description = norm(inputDesc.value);
    if (!name) { alert("Name is required"); return; }

    const now = new Date();
    const payload = {
        name,
        description,
        status: "Pending",     
        dueDate: now.toISOString(),
        createdAt: now.toISOString(),
        approved: false
    };

    try {
        const created = await apiPost(payload);
        const newTask = {
            id: created.id,
            name: created.name ?? name,
            description: created.description ?? description,
            status: created.status ?? payload.status,
            dueDate: created.dueDate ?? payload.dueDate,
            createdAt: created.createdAt ?? payload.createdAt,
            approved: created.approved ?? payload.approved
        };
        tasks.unshift(newTask);
        appendTaskCard(newTask);
        updateCount();
        inputName.value = "";
        inputDesc.value = "";
        if (inputReporter) inputReporter.value = "";
        showAddForm(false);
        toast("Task created");
    } catch (err) {
        console.error(err);
        alert("Error creating task");
    }
});

document.addEventListener("click", async (e) => {
    const a = e.target.closest("a");
    if (!a) return;

    // DELETE
    if (a.id?.startsWith("delete-")) {
        e.preventDefault();
        const id = Number(a.dataset.id || a.id.split("-")[1]);
        if (!id || !confirm(`Delete task #${id}?`)) return;

        try {
            await apiDelete(id);
            const idx = tasks.findIndex(t => t.id === id);
            if (idx >= 0) tasks.splice(idx, 1);
            document.getElementById(`Task-${id}`)?.remove();
            updateCount();
            toast("Task deleted");
        } catch (err) {
            console.error(err);
            alert("Error deleting task");
        }
        return;
    }

    // EDIT
    if (a.id?.startsWith("edit-")) {
        e.preventDefault();
        const id = Number(a.dataset.id || a.id.split("-")[1]);
        const t = tasks.find(x => x.id === id);
        if (!t) { alert("Task not found"); return; }

        const newName = prompt("Name:", t.name ?? "") ?? t.name;
        const newDesc = prompt("Description:", t.description ?? "") ?? t.description;
        const newStatus = prompt("Status (Open/Closed/Pending/True/False):", t.status ?? "Pending") ?? t.status;

        const payload = {
            id: t.id,
            name: newName,
            description: newDesc,
            status: newStatus,
            dueDate: t.dueDate,
            createdAt: t.createdAt,
            approved: t.approved
        };

        try {
            const updated = await apiPut(id, payload);
            Object.assign(t, updated);
            replaceTaskCard(t);
            toast("Task updated");
        } catch (err) {
            console.error(err);
            alert("Error updating task");
        }
        return;
    }
});

updateCount();