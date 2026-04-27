//const a = document.querySelector(".forgot-link");
//a.addEventListener("click", async (event) =>
//{
//    event.preventDefault();
//    const email = document.querySelector(".email-field");
//    console.log(email.value);
//    if (email.value == "" || !(/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value))) {
//        setMessage("Моля попълнете полето за имейл", "error");
//        email.focus();
//    } else {
//        setMessage("Изпращане на имейл...", "info");
//        a.style.pointerEvents = "none"; // disable link

//        const res = await fetch("/User/ForgotPassword", {
//            method: "POST",
//            headers: {
//                "Content-Type": "application/json"
//            },
//            body: JSON.stringify(email.value)
//        });

//        const resBody = await res.json();
//        setMessage(resBody.msg, resBody.type);
//    }

//});

//function setMessage(msg, type) {
//    const formGroup = document.querySelector('.em');
//    const span = formGroup.querySelector("span");

//    span.innerText = msg;
//    if (type == "error") {
//        span.classList.add("text-error");
//        span.classList.remove("text-success");
//        span.classList.remove("text-primary");
//    } else if (type == "success") {
//        span.classList.add("text-success");
//        span.classList.remove("text-error");
//        span.classList.remove("text-primary");
//    } else if (type == "info") {
//        span.classList.add("text-primary");
//        span.classList.remove("text-error");
//        span.classList.remove("text-success");
//    }
//}

////document.querySelector(".btn-google").addEventListener("click", async () =>
////{
////    const res = await fetch(`/User/ExternalLogin&provider=Google`, { method: "GET" });
////})

const loginForm = document.querySelector('form');
loginForm.addEventListener('submit', async function (e) {
    e.preventDefault();
    const btn = e.target.querySelector('button[type="submit"]');
    const btnText = btn.querySelector('span');
    const errorContainer = document.getElementById('login-error-container');
    const errorMessage = document.getElementById('login-error-message');

    // Loading state
    const originalText = btnText.innerText;
    btnText.innerText = 'ОБРАБОТКА...';
    btn.disabled = true;
    errorContainer.classList.add('hidden');

    const formData = new FormData(loginForm);


    const res = await fetch('/User/Login', {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken':
                document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    });

    const data = await res.json();
    if (!data.success) {
        errorMessage.innerText = 'Невалиден имейл или парола. Моля, опитайте отново.';
        errorContainer.classList.remove('hidden');

        // Reset button
        btnText.innerText = originalText;
        btn.disabled = false;

        // Subtle shake effect
        btn.closest('.glass-panel').classList.add('animate-shake');
        setTimeout(() => btn.closest('.glass-panel').classList.remove('animate-shake'), 500);

    } else {
        window.location.href = "/";
    }


});

