//document.addEventListener("DOMContentLoaded", function () {
//    // Toggle password visibility for each toggle button
//    document.querySelectorAll(".toggle-password").forEach(function (btn) {
//        btn.addEventListener("click", function (e) {
//            var targetSelector = btn.getAttribute("data-target");
//            if (!targetSelector) return;
//            var input = document.querySelector(targetSelector);
//            if (!input) return;

//            var icon = btn.querySelector("i");
//            if (input.type === "password") {
//                input.type = "text";
//                if (icon) {
//                    icon.classList.remove("bi-eye");
//                    icon.classList.add("bi-eye-slash");
//                }
//                btn.setAttribute("aria-pressed", "true");
//            } else {
//                input.type = "password";
//                if (icon) {
//                    icon.classList.remove("bi-eye-slash");
//                    icon.classList.add("bi-eye");
//                }
//                btn.setAttribute("aria-pressed", "false");
//            }
//            try {
//                input.focus();
//                var val = input.value;
//                input.value = "";
//                input.value = val;
//            } catch (ex) {
//                // ignore
//            }
//        });
//    });

//    let newPw = document.getElementById("newPassword");
//    let confirmPw = document.getElementById("confirmPassword");
//    let resetBtn = document.querySelector(".reset-btn");
//    if (newPw && confirmPw) {
//        function showMatchHint() {
//            var span = confirmPw.closest(".form-group")?.querySelector("span");
//            if (!span) return;
//            if (confirmPw.value.length === 0) {
//                span.style.outline = "1px solid red";
//                span.innerText = "";
//                span.className = "text-danger";
//                resetBtn.disabled = true;
//                return;
//            }
//            if (newPw.value === confirmPw.value) {
//                span.innerText = "";
//                span.style.outline = "1px solid green";
//                span.className = "text-success";
//                resetBtn.disabled = false;
//            } else {
//                span.style.outline = "1px solid red";
//                span.innerText = "Паролите не съвпадат.";
//                span.className = "text-danger";
//                resetBtn.disabled = true;
//            }
//        }
//        newPw.addEventListener("input", showMatchHint);
//        confirmPw.addEventListener("input", showMatchHint);
//    }

//    //document.getElementById('resetPasswordForm').addEventListener('submit', async function (event) {
//    //    event.preventDefault();
//    //    let antiToken = document.querySelector('input[name="__RequestVerificationToken"]');

//    //    const formData = {
//    //        Token: document.getElementById('token').value,
//    //        Email: document.getElementById('email').value,
//    //        NewPassword: document.getElementById('newPassword').value,
//    //    };
//    //    console.log(formData.Token);
//    //    try {
//    //        const response = await fetch('/user/resetpassword', {
//    //            method: 'POST',
//    //            headers: {
//    //                'Content-Type': 'application/json',
//    //                'RequestVerificationToken': antiToken.value
//    //            },
//    //            body: JSON.stringify(formData)
//    //        });
//    //        const result = await response.json();
//    //        if (confirm(result.message)) {
//    //            window.location.href = "/User/Login";
//    //        }
//    //    } catch (error) {
//    //        console.error('Error:', error);
//    //        alert('Възникна грешка при обработката на заявката.');
//    //    }
//    //});
//});

const passwordInput = document.getElementById('password');
const confirmInput = document.getElementById('confirm-password');
const submitBtn = document.getElementById('submit-btn');
const statusContainer = document.getElementById('status-message-container');

document.querySelectorAll('.view-password').forEach(btn => {
    btn.addEventListener('click', () => {
        const wrapper = btn.closest('.relative');
        const input = wrapper.querySelector('input');
        const icon = btn.querySelector('span');

        const isHidden = input.type === 'password';
        input.type = isHidden ? 'text' : 'password';

        icon.textContent = isHidden ? 'visibility_off' : 'visibility';
    });
});

const checklistItems = {
    length: document.querySelector('li:nth-child(1)'),
    symbols: document.querySelector('li:nth-child(2)'),
    match: document.querySelector('li:nth-child(3)')
};

function updateChecklist(item, isValid) {
    const icon = item.querySelector('.material-symbols-outlined');
    if (isValid) {
        icon.innerText = 'check_circle';
        icon.classList.remove('text-outline');
        icon.classList.add('text-tertiary');
        icon.style.fontVariationSettings = "'FILL' 1";
        item.classList.remove('text-on-surface-variant/70');
        item.classList.add('text-on-surface');
    } else {
        icon.innerText = 'radio_button_unchecked';
        icon.classList.add('text-outline');
        icon.classList.remove('text-tertiary');
        icon.style.fontVariationSettings = "'FILL' 0";
        item.classList.add('text-on-surface-variant/70');
        item.classList.remove('text-on-surface');
    }
}

function validate() {
    const pass = passwordInput.value;
    const conf = confirmInput.value;

    const hasLength = pass.length >= 8;
    const hasSymbols = /[!@#$%^&*(),.?":{}|<>]/.test(pass);
    const doesMatch = pass === conf && pass !== '';

    updateChecklist(checklistItems.length, hasLength);
    updateChecklist(checklistItems.symbols, hasSymbols);
    updateChecklist(checklistItems.match, doesMatch);

    return hasLength && hasSymbols && doesMatch;
}

[passwordInput, confirmInput].forEach(input => {
    input.addEventListener('input', validate);
});

document.querySelector('form').addEventListener('submit', async (e) => {
    e.preventDefault();
    let antiToken = document.querySelector('input[name="__RequestVerificationToken"]');


    if (!validate()) return;

    // Loading State
    const originalContent = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = `<span class="animate-spin material-symbols-outlined">progress_activity</span> <span>ОБРАБОТКА...</span>`;
    submitBtn.classList.add('opacity-80');
    statusContainer.classList.add('hidden');

    let fd = new FormData(document.querySelector('form'));

    const res = await fetch('/user/resetpassword', {
        method: 'POST',
        headers: {
            'RequestVerificationToken': antiToken.value
        },
        body: fd
    });

    const resBody = await res.json();

    submitBtn.disabled = false;
    submitBtn.innerHTML = originalContent;
    submitBtn.classList.remove('opacity-80');

    
    statusContainer.classList.remove('hidden');

    if (resBody.success) {
        statusContainer.innerHTML = `
          <div class="bg-tertiary-container/20 border border-tertiary/30 p-4 rounded-lg flex items-center gap-3">
            <span class="material-symbols-outlined text-tertiary">check_circle</span>
            <p class="text-sm text-tertiary font-bold uppercase tracking-tight">Паролата беше сменена успешно. Можете да влезете с новата си парола.</p>
          </div>`;
    } else {
        statusContainer.innerHTML = `
          <div class="bg-primary-container/20 border border-primary-container/30 p-4 rounded-lg flex items-center gap-3">
            <span class="material-symbols-outlined text-primary-container">error</span>
            <p class="text-sm text-primary-container font-bold uppercase tracking-tight">Грешка при смяна на паролата. Възможно е линкът да е изтекъл или да е невалиден.</p>
          </div>`;
    }

});