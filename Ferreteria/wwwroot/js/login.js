document.addEventListener("DOMContentLoaded", function () {
    // Mostrar / ocultar contraseña
    document.querySelectorAll(".toggle-password-btn").forEach((btn) => {
        btn.addEventListener("click", function () {
            const targetId = this.getAttribute("data-target");
            const input = document.getElementById(targetId);
            const icon = this.querySelector("i");

            if (!input || !icon) return;

            const isPassword = input.type === "password";
            input.type = isPassword ? "text" : "password";
            icon.className = isPassword ? "fa-regular fa-eye" : "fa-regular fa-eye-slash";
        });
    });

    // Función para cambiar entre formularios
    function switchForm(formId) {
        document.querySelectorAll(".form").forEach((form) => {
            form.classList.remove("active");
        });
        const target = document.getElementById(formId);
        if (target) {
            target.classList.add("active");
        }
    }

    // Event listener para cambiar de formularios
    document.querySelectorAll(".switch-form, .forgot-link").forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const formId = this.getAttribute("data-form");
            switchForm(formId);
        });
    });

    // ---------------- LOGIN FORM ---------------- //
    const validate = new JustValidate("#signupForm");

    validate
        .addField("#email", [
            {
                rule: "required",
                errorMessage: "El usuario es obligatorio"
            }
        ])
        .addField("#password", [
            {
                rule: "required",
                errorMessage: "La contraseña es obligatoria"
            }
        ])
        .onSuccess(async (event) => {
            event.preventDefault();

            const data = {
                Email: document.getElementById("email").value,
                Password: document.getElementById("password").value,
                RememberMe: document.getElementById("rememberMe")?.checked ?? false
            };

            try {
                const response = await axios.post("/Auth/Login", data);

                if (response.data.success) {
                    Swal.fire({
                        toast: true,
                        position: "top-end",
                        icon: "success",
                        title: "Inicio de sesión exitoso",
                        showConfirmButton: false,
                        timer: 2000
                    });

                    setTimeout(() => {
                        window.location.href = "/Home/Index";
                    }, 2000);
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: response.data.errorMessage || response.data.message || "Credenciales inválidas."
                    });
                }
            } catch (err) {
                const resData = err.response?.data;
                Swal.fire({
                    icon: "error",
                    title: "Error del servidor",
                    text: resData?.errorMessage || resData?.message || "No se pudo conectar con el servidor."
                });
            }
        });

    // ---------------- REGISTER FORM ---------------- //
    const validateRegisterForm = new JustValidate("#registerFormElement");

    validateRegisterForm
        .addField("#UserName", [
            {
                rule: "required",
                errorMessage: "El usuario es obligatorio"
            }
        ])
        .addField("#registerEmail", [
            {
                rule: "required",
                errorMessage: "El correo es obligatorio"
            },
            {
                rule: "email",
                errorMessage: "No es un correo válido."
            }
        ])
        .addField("#registerPassword", [
            {
                rule: "required",
                errorMessage: "La contraseña es requerida"
            },
            {
                validator: (value) =>
                    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_\-+=<>.,;:{}[\]|/~]).{8,}$/.test(value),
                errorMessage: "Debe tener mayúscula, minúscula, número y símbolo"
            }
        ])
        .addField("#confirmPassword", [
            {
                validator: (value, fields) => {
                    return value === fields["#registerPassword"]?.elem?.value;
                },
                errorMessage: "Las contraseñas no coinciden"
            }
        ])
        .onSuccess(async (event) => {
            event.preventDefault();

            const data = {
                Username: document.getElementById("UserName").value,
                Email: document.getElementById("registerEmail").value,
                Password: document.getElementById("registerPassword").value
            };

            try {
                const response = await axios.post("/Auth/Register", data);

                if (response.data.success) {
                    Swal.fire({
                        toast: true,
                        position: "top-end",
                        icon: "success",
                        title: "Cuenta creada exitosamente. Revisa tu correo para confirmarla.",
                        showConfirmButton: false,
                        timer: 2000
                    });

                    setTimeout(() => {
                        window.location.href = "/";
                    }, 2000);
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: response.data.errorMessage || response.data.message || "Credenciales inválidas."
                    });
                }
            } catch (err) {
                const resData = err.response?.data;
                const mensaje = resData?.errorMessage || resData?.message || "No se pudo conectar con el servidor.";
                Swal.fire({
                    icon: "error",
                    title: "Error del servidor",
                    text: mensaje
                });
            }
        });

    // ---------------- FORGET PASSWORD FORM ---------------- //
    const validateForgetForm = new JustValidate("#resetPasswordForm");

    validateForgetForm
        .addField("#resetEmail", [
            {
                rule: "required",
                errorMessage: "El correo es obligatorio"
            },
            {
                rule: "email",
                errorMessage: "No es un correo válido."
            }
        ])
        .onSuccess(async (event) => {
            event.preventDefault();

            const data = {
                Email: document.getElementById("resetEmail").value
            };

            try {
                const response = await axios.post("/Auth/ForgotPassword", data);

                if (response.data.success) {
                    Swal.fire({
                        toast: true,
                        position: "top-end",
                        icon: "success",
                        title: "Se ha enviado un enlace con el que puedas restaurar tu contraseña. Revisa tu correo para confirmarla.",
                        showConfirmButton: false,
                        timer: 2000
                    });

                    setTimeout(() => {
                        window.location.href = "/";
                    }, 2000);
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: response.data.errorMessage || response.data.message || "Credenciales inválidas."
                    });
                }
            } catch (err) {
                const resData = err.response?.data;
                const mensaje = resData?.errorMessage || resData?.message || "No se pudo conectar con el servidor.";
                Swal.fire({
                    icon: "error",
                    title: "Error del servidor",
                    text: mensaje
                });
            }
        });
});
