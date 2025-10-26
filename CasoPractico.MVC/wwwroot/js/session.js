(function () {
    const user = JSON.parse(localStorage.getItem("cp_user") || "null");
    const hasUser = !!user;

    const loginLink = document.getElementById("navLogin");
    const homeLink = document.getElementById("navHome");

    if (loginLink) {

        if (hasUser) {
            loginLink.textContent = "Logout";
            loginLink.classList.remove("btn-outline-light");
            loginLink.classList.add("btn-outline-warning");
            loginLink.href = "#";

            loginLink.addEventListener("click", (e) => {
                e.preventDefault();
                localStorage.removeItem("cp_user");
                window.location.replace("/Home/Login");
            });

            if (homeLink) homeLink.classList.remove("d-none");
        } else {

            loginLink.textContent = "Login";
            loginLink.classList.add("btn-outline-light");
            loginLink.classList.remove("btn-outline-warning");
            loginLink.href = "/Home/Login";

            if (homeLink) homeLink.classList.add("d-none");
        }
    }

    const onLoginPage = window.location.pathname.toLowerCase().includes("/home/login");
    if (!hasUser && !onLoginPage) {
        window.location.replace("/Home/Login");
    }
})();
