document.addEventListener("DOMContentLoaded", () => {
    console.log("Login JS loaded");

    const loginBtn = document.getElementById("loginBtn");
    if (!loginBtn) return;

    loginBtn.addEventListener("click", async (e) => {
        e.preventDefault();

        const email = document.getElementById("loginEmail").value.trim();
        const password = document.getElementById("loginPassword").value.trim();

        if (!email || !password) {
            alert("Por favor ingrese email y contraseña");
            return;
        }

        try {
            const res = await fetch("https://localhost:7281/api/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password })
            });

            if (!res.ok) {
                alert("Credenciales invalidas");
                return;
            }

            const user = await res.json();

            //roles
            const roleRes = await fetch(`https://localhost:7281/api/users/${user.userId}/roles`);
            const roles = roleRes.ok ? await roleRes.json() : [];

            localStorage.setItem("cp_user", JSON.stringify(user));
            localStorage.setItem("cp_roles", JSON.stringify(roles.map(r => r.roleName)));

            alert("Bienvenido :)");
            window.location.href = "/Home/Index";
        } catch (err) {
            console.error(err);
            alert("Error de conexion");
        }
    });
});
