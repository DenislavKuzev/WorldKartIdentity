document.getElementById("registerForm").addEventListener("submit", function (e) {
        let isValid = true;

    const username = document.getElementById("UserName");
    const email = document.getElementById("Email");
    const password = document.getElementById("Password");
    const repeatPassword = document.getElementById("RepeatPassword");

    clearErrors();

    // USERNAME: 3–20 символа, букви, цифри, _
    if (!/^[a-zA-Z0-9_]{3, 20}$/.test(username.value)) {
        showError(username, "Потребителското име трябва да е 3–20 символа и да съдържа само букви, цифри и _!");
    isValid = false;
    }

    // EMAIL
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
        showError(email, "Невалиден имейл адрес!");
    isValid = false;
    }

    // PASSWORD: мин. 8 символа, 1 главна, 1 малка, 1 цифра
    if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(password.value)) {
        showError(password, "Паролата трябва да е поне 8 символа, с главна, малка буква и цифра!");
    isValid = false;
    }

    // REPEAT PASSWORD
    if (password.value !== repeatPassword.value) {
        showError(repeatPassword, "Паролите не съвпадат!");
    isValid = false;
    }

    if (!isValid) {
        e.preventDefault();
    }
});

    function showError(input, message) {
    const span = input.parentElement.querySelector(".text-danger");
    span.innerText = message;
}

    function clearErrors() {
        document.querySelectorAll(".text-danger").forEach(span => span.innerText = "");
}
