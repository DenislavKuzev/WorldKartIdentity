//    const form = document.getElementById("registrationForm");

//    const username = document.getElementById("UserName");
//    const email = document.getElementById("Email");
//    const password = document.getElementById("Password");
//    const repeatPassword = document.getElementById("RepeatPassword");

//    // ========== EVENTS ==========
//    username.addEventListener("input", validateUsername);
//    email.addEventListener("input", validateEmail);
//    password.addEventListener("input", validatePassword);
//    repeatPassword.addEventListener("input", validateRepeatPassword);

//form.addEventListener("submit", function (e) {
//    e.preventDefault();

//    const isFormValid =
//    validateUsername() &
//    validateEmail() &
//    validatePassword() &
//    validateRepeatPassword();

//    if (isFormValid) {
//        handleSubmition(e);
//    }
//});

//    // ========== VALIDATION FUNCTIONS ==========
//    function validateUsername() {
//    const value = username.value.trim();

//    if (!/^[a-zA-Z0-9_]{3,20}$/.test(value)) {
//        showError(username, "Потребителското име трябва да е 3–20 символа и да съдържа само букви, цифри и _!");
//    return false;
//    }

//    clearError(username);
//    return true;
//}

//    function validateEmail() {
//    const value = email.value.trim();

//    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
//        showError(email, "Невалиден имейл адрес!");
//    return false;
//    }

//    clearError(email);
//    return true;
//}

//    function validatePassword() {
//    const value = password.value;

//    if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(value)) {
//        showError(password, "Паролата трябва да съдържа мин. 8 символа, главна, малка буква и цифра");
//    return false;
//    }

//    clearError(password);
//    return true;
//}

//    function validateRepeatPassword() {
//    if (repeatPassword.value !== password.value || repeatPassword.value === "") {
//        showError(repeatPassword, "Паролите не съвпадат");
//    return false;
//    }

//    clearError(repeatPassword);
//    return true;
//}

//async function handleSubmition(e) {
//    const fd = new FormData(form);
//    const reqbody = {
//        userName: fd.get("UserName"),
//        email: fd.get("Email"),
//        password: fd.get("Password"),
//        repeatPassword: fd.get("RepeatPassword")
//    };
//    console.log(fd.get("UserName"));
//    console.log(fd.get("Email"))
//    let res = await fetch("/User/Registration", {
//        method: "POST",
//        headers: {
//            "Content-Type": "application/json"
//        },
//        body: JSON.stringify(reqbody)
//    });

//    let resBody = await res.json();
//    if (!resBody.success) {
//        let splitMsg = resBody.message.split(".");
//        console.log(splitMsg[0]);
//        if (splitMsg[1].includes("E")) {
//            showError(email, splitMsg[0]);
//        } else if (splitMsg[1].includes("U")) {
//            showError(username, splitMsg[0]);
//        }
//    } else {
//        window.location.href = "/";
//    }

//}

//    // ========== HELPERS ==========
//    function showError(input, message) {
//    const span = input.parentElement.querySelector(".text-danger");
//    span.innerText = message;
//    input.classList.add("input-error");
//}

//    function clearError(input) {
//    const span = input.parentElement.querySelector(".text-danger");
//    span.innerText = "";
//    input.classList.remove("input-error");
//}

const signupForm = document.querySelector('.signup');
const submitBtn = signupForm.querySelector('button[type="submit"]');
const statusMsg = document.getElementById('form-status');

const validations = {
    UserName: {
        validate: (val) => /^[a-zA-Z0-9_]{3,20}$/.test(val),
        errorMsg: "3-20 знака: букви, цифри или долна черта."
    },
    Email: {
        validate: (val) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val),
        errorMsg: "Моля, въведете валиден имейл адрес."
    },
    Password: {
        validate: (val) => /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(val),
        errorMsg: "Мин. 8 знака, главна, малка буква и цифра."
    },
    'RepeatPassword': {
        validate: (val) => val === document.getElementById('password').value,
        errorMsg: "Паролите не съвпадат."
    }
};

function showError(id, msg) {
    const errorEl = document.getElementById(`${id}-error`);
    console.log(errorEl);
    if (errorEl) {
        errorEl.textContent = msg;
        errorEl.classList.remove('hidden');
    }
}

function clearErrors() {
    document.querySelectorAll('[id$="-error"]').forEach(el => {
        el.classList.add('hidden');
        el.textContent = '';
    });
    statusMsg.classList.add('hidden');
}

signupForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    clearErrors();

    const fd = new FormData(signupForm);
    const data = Object.fromEntries(fd.entries());
    let isValid = true;

    for (const [key, rules] of Object.entries(validations)) {
        if (!rules.validate(data[key])) {
            console.log(data[key]);
            showError(key, rules.errorMsg);
            isValid = false;
        }
    }

    if (!isValid) return;

    // UI State: Loading
    const originalBtnContent = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="animate-spin material-symbols-outlined">progress_activity</span><span>Обработка...</span>';    const reqbody = {
            userName: fd.get("UserName"),
            email: fd.get("Email"),
            password: fd.get("Password"),
            repeatPassword: fd.get("RepeatPassword")
    };

        const response = await fetch('/User/Registration', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(reqbody)
        });

    const resBody = await response.json();
    console.log(resBody);

        if (resBody.success) {
            statusMsg.textContent = "Успешна регистрация! Пренасочване...";
            statusMsg.className = "mb-4 p-4 rounded-xl text-sm font-medium bg-green-500/20 text-green-400 border border-green-500/30";
            statusMsg.classList.remove('hidden');
            signupForm.reset();
            setTimeout(() => {
                window.location.href = "/";
            }, 400)
        }
        else
        {
            statusMsg.textContent = resBody.message || "Възникна технически проблем.";
            statusMsg.className = "mb-4 p-4 rounded-xl text-sm font-medium bg-rose-500/20 text-rose-400 border border-rose-500/30";
            statusMsg.classList.remove('hidden');
            submitBtn.innerHTML = originalBtnContent;
        }
    
});