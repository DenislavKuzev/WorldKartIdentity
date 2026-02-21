const a = document.querySelector(".forgot-link");
a.addEventListener("click", async (event) =>
{
    event.preventDefault();
    const email = document.querySelector(".email-field");
    console.log(email.value);
    if (email.value == "" || !(/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value))) {
        setMessage("Моля попълнете полето за имейл", "error");
        email.focus();
    } else {
        setMessage("Изпращане на имейл...", "info");
        a.style.pointerEvents = "none"; // disable link

        const res = await fetch("/User/ForgotPassword", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(email.value)
        });

        const resBody = await res.json();
        setMessage(resBody.msg, resBody.type);
    }

});

function setMessage(msg, type) {
    const formGroup = document.querySelector('.em');
    const span = formGroup.querySelector("span");

    span.innerText = msg;
    if (type == "error") {
        span.classList.add("text-error");
        span.classList.remove("text-success");
        span.classList.remove("text-primary");
    } else if (type == "success") {
        span.classList.add("text-success");
        span.classList.remove("text-error");
        span.classList.remove("text-primary");
    } else if (type == "info") {
        span.classList.add("text-primary");
        span.classList.remove("text-error");
        span.classList.remove("text-success");
    }
}