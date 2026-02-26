    const form = document.getElementById("registrationForm");

    const username = document.getElementById("UserName");
    const email = document.getElementById("Email");
    const password = document.getElementById("Password");
    const repeatPassword = document.getElementById("RepeatPassword");

    // ========== EVENTS ==========
    username.addEventListener("input", validateUsername);
    email.addEventListener("input", validateEmail);
    password.addEventListener("input", validatePassword);
    repeatPassword.addEventListener("input", validateRepeatPassword);

form.addEventListener("submit", function (e) {
    e.preventDefault();

    const isFormValid =
    validateUsername() &
    validateEmail() &
    validatePassword() &
    validateRepeatPassword();

    if (isFormValid) {
        handleSubmition(e);
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

async function handleSubmition(e) {
    const fd = new FormData(form);
    const reqbody = {
        userName: fd.get("UserName"),
        email: fd.get("Email"),
        password: fd.get("Password"),
        repeatPassword: fd.get("RepeatPassword")
    };
    console.log(fd.get("UserName"));
    console.log(fd.get("Email"))
    let res = await fetch("/User/Registration", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(reqbody)
    });

    let resBody = await res.json();
    if (!resBody.success) {
        let splitMsg = resBody.message.split(".");
        console.log(splitMsg[0]);
        if (splitMsg[1].includes("E")) {
            showError(email, splitMsg[0]);
        } else if (splitMsg[1].includes("U")) {
            showError(username, splitMsg[0]);
        } 
    } else {
        window.location.href = "/";
    }

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
