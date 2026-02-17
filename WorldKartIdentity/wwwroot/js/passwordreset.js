document.addEventListener("DOMContentLoaded", function () {
    // Toggle password visibility for each toggle button
    document.querySelectorAll(".toggle-password").forEach(function (btn) {
        btn.addEventListener("click", function (e) {
            var targetSelector = btn.getAttribute("data-target");
            if (!targetSelector) return;
            var input = document.querySelector(targetSelector);
            if (!input) return;

            var icon = btn.querySelector("i");
            if (input.type === "password") {
                input.type = "text";
                if (icon) {
                    icon.classList.remove("bi-eye");
                    icon.classList.add("bi-eye-slash");
                }
                btn.setAttribute("aria-pressed", "true");
            } else {
                input.type = "password";
                if (icon) {
                    icon.classList.remove("bi-eye-slash");
                    icon.classList.add("bi-eye");
                }
                btn.setAttribute("aria-pressed", "false");
            }
            try {
                input.focus();
                var val = input.value;
                input.value = "";
                input.value = val;
            } catch (ex) {
                // ignore
            }
        });
    });

    let newPw = document.getElementById("newPassword");
    let confirmPw = document.getElementById("confirmPassword");
    let resetBtn = document.querySelector(".reset-btn");
    if (newPw && confirmPw) {
        function showMatchHint() {
            var span = confirmPw.closest(".form-group")?.querySelector("span");
            if (!span) return;
            if (confirmPw.value.length === 0) {
                span.style.outline = "1px solid red";
                span.innerText = "";
                span.className = "text-danger";
                resetBtn.disabled = true;
                return;
            }
            if (newPw.value === confirmPw.value) {
                span.innerText = "";
                span.style.outline = "1px solid green";
                span.className = "text-success";
                resetBtn.disabled = false;
            } else {
                span.style.outline = "1px solid red";
                span.innerText = "Паролите не съвпадат.";
                span.className = "text-danger";
                resetBtn.disabled = true;
            }
        }
        newPw.addEventListener("input", showMatchHint);
        confirmPw.addEventListener("input", showMatchHint);
    }

    document.getElementById('resetPasswordForm').addEventListener('submit', async function (event) {
        event.preventDefault();
        let antiToken = document.querySelector('input[name="__RequestVerificationToken"]');

        const formData = {
            Token: document.getElementById('token').value,
            Email: document.getElementById('email').value,
            NewPassword: document.getElementById('newPassword').value,
        };
        console.log(formData.Token);
        try {
            const response = await fetch('/User/ResetPassword', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': antiToken.value
                },
                body: JSON.stringify(formData)
            });
            const result = await response.json();

            if (response.ok) {
                const result = await response.json();
                alert('Паролата е успешно нулирана!');
                window.location.href = '/User/Login'; // Redirect to login page
            } else {
                const error = await response.json();
                alert(`Грешка: ${error.message || 'Неуспешно нулиране на паролата.'}`);
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Възникна грешка при обработката на заявката.');
        }
    });
});