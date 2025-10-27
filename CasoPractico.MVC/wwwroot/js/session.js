document.addEventListener("DOMContentLoaded", () => {
    const user = JSON.parse(localStorage.getItem("cp_user") || "null");
    const roles = JSON.parse(localStorage.getItem("cp_roles") || "[]");
    const hasUser = !!user;

    const $ = (id) => document.getElementById(id);
    const navLogin = $("navLogin");
    const navHome = $("navHome");
    const navApprovals = $("navApprovals");
    const navRoles = $("navRoles");
    const navLogout = $("navLogout");

    document.body.classList.remove("preauth");

    const isManagerOrAdmin = () => {
        const set = new Set((roles || []).map(r => (r || "").toLowerCase()));
        return set.has("manager") || set.has("system admin") || set.has("admin");
    };

    function setLoggedOutUI() {
        navLogin?.classList.remove("d-none");
        navHome?.classList.add("d-none");
        navApprovals?.classList.add("d-none");
        navRoles?.classList.add("d-none");
        navLogout?.classList.add("d-none");
    }

    function setLoggedInUI() {
        navLogin?.classList.add("d-none");
        navHome?.classList.remove("d-none");
        navLogout?.classList.remove("d-none");
        if (isManagerOrAdmin()) {
            navApprovals?.classList.remove("d-none");
            navRoles?.classList.remove("d-none");
        } else {
            navApprovals?.classList.add("d-none");
            navRoles?.classList.add("d-none");
        }
    }

    // estado
    if (hasUser) setLoggedInUI(); else setLoggedOutUI();

    const path = window.location.pathname.toLowerCase();
    const onLogin = path.includes("/home/login");
    if (!hasUser && !onLogin) {
        window.location.replace("/Home/Login");
        return;
    }

    // Logout
    navLogout?.addEventListener("click", (e) => {
        e.preventDefault();
        localStorage.removeItem("cp_user");
        localStorage.removeItem("cp_roles");
        setLoggedOutUI();
        window.location.replace("/Home/Login");
    });

    [navLogin, navHome, navApprovals, navRoles, navLogout].forEach(a => a?.classList.remove("active"));
    if (onLogin) navLogin?.classList.add("active");
    else if (path.startsWith("/home/index")) navHome?.classList.add("active");
    else if (path.startsWith("/approvals")) navApprovals?.classList.add("active");
    else if (path.startsWith("/roles")) navRoles?.classList.add("active");
});