const initiateBtn = document.getElementById('initiateBtn');
const processingOverlay = document.getElementById('processingOverlay');
const processingText = document.getElementById('processingText');
const analysisContent = document.getElementById('analysisContent');
const victoryDetails = document.getElementById('victoryDetails');

const technicalAdvisory = document.getElementById('technicalAdvisory');
const winnerName = document.getElementById('winnerName');

initiateBtn.addEventListener('click', async () => {
    // Start Loading State
    processingOverlay.classList.remove('hidden');
    analysisContent.classList.add('opacity-50');
    victoryDetails.classList.add('opacity-0', 'scale-95');

    processingText.innerText = "Анализ на траекториите...";
    let fd = new FormData();
    fd.append("challengerTrajId", window.trajContext.challengerId);
    fd.append("challengedTrajId", window.trajContext.opponentId);


    const res = await fetch('/tracks/challenge-result', {
        method: 'POST',
        body: fd
    })
    const resBody = await res.json();
    const analysis = JSON.parse(resBody.analysis);
    console.log(analysis);

    // Populate and Show Results
    processingOverlay.classList.add('hidden');
    // initiateLabel.innerText = "INITIATE";
    analysisContent.classList.remove('opacity-50');

    // Set Data
    technicalAdvisory.innerHTML = `<span class="text-rose-600 font-bold font-display uppercase mr-2 tracking-tighter">АНАЛИЗ:</span> ${analysis.explanation}`;
    technicalAdvisory.classList.remove('text-slate-500', 'italic');
    technicalAdvisory.classList.add('text-slate-300');

    winnerName.innerText = analysis.winner;

    // Animate In
    victoryDetails.classList.remove('opacity-0', 'scale-95');
    victoryDetails.classList.add('opacity-100', 'scale-100');


});