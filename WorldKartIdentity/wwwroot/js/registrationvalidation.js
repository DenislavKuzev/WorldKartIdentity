    const form = document.getElementById("registrationForm");

    const username = document.getElementById("UserName");
    const email = document.getElementById("Email");
    const password = document.getElementById("Password");
    const repeatPassword = document.getElementById("RepeatPassword");

    // ========== EVENTS ==========
    username.addEventListener("blur", validateUsername);
    email.addEventListener("blur", validateEmail);
    password.addEventListener("blur", validatePassword);
    repeatPassword.addEventListener("blur", validateRepeatPassword);

    form.addEventListener("submit", function (e) {
    const isFormValid =
    validateUsername() &
    validateEmail() &
    validatePassword() &
    validateRepeatPassword();

    if (!isFormValid) {
        e.preventDefault();
    }
});

    // ========== VALIDATION FUNCTIONS ==========
    function validateUsername() {
    const value = username.value.trim();

    if (!/^[a-zA-Z0-9_]{3,20}$/.test(value)) {
        showError(username, "Потребителското име трябва да е 3–20 символа и да съдържа само букви, цифри и _!");
    return false;
    }

    clearError(username);
    return true;
}

    function validateEmail() {
    const value = email.value.trim();

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
        showError(email, "Невалиден имейл адрес!");
    return false;
    }

    clearError(email);
    return true;
}

    function validatePassword() {
    const value = password.value;

    if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(value)) {
        showError(password, "Паролата трябва да съдържа мин. 8 символа, главна, малка буква и цифра");
    return false;
    }

    clearError(password);
    return true;
}

    function validateRepeatPassword() {
    if (repeatPassword.value !== password.value || repeatPassword.value === "") {
        showError(repeatPassword, "Паролите не съвпадат");
    return false;
    }

    clearError(repeatPassword);
    return true;
}

    // ========== HELPERS ==========
    function showError(input, message) {
    const span = input.parentElement.querySelector(".text-danger");
    span.innerText = message;
    input.classList.add("input-error");
}

    function clearError(input) {
    const span = input.parentElement.querySelector(".text-danger");
    span.innerText = "";
    input.classList.remove("input-error");
}
