window.process = window.process || { env: { NODE_ENV: "production" } }
let anno = null;
let user = null;
const btnClear = document.getElementById("btnClear");

document.addEventListener("DOMContentLoaded", async () => {
    const userRes = await fetch('/user/me');
    user = await userRes.json();
    const img = document.querySelector(".traj-image");
    if (user.authenticated) {
        anno = Annotorious.init({
            image: img
        });

        const userInfo = {
            id: `user:${user.userId}`,
            displayName: user.username,
        };
        anno.setAuthInfo(userInfo);

        console.log(window.trajectoryContext);
        anno.setAnnotations(window.trajectoryContext.annotations);

        
        attachEvents();
    }
});

function attachEvents() {
    anno.on('createAnnotation', async (a) => {
        await sendAnnotationRequest('/Track/CreateTrackAnnotation', a);
    });

    anno.on('updateAnnotation', async (a) => {
        await sendAnnotationRequest('/Track/UpdateTrackAnnotation', a);
    })

    anno.on('deleteAnnotation', async (a) => {
        await sendAnnotationRequest('/Track/DeleteTrackAnnotation', a);
    });

}
async function sendAnnotationRequest(url, a) {

        const params = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                trackId: window.trajectoryContext.trackId,
                trajectoryId: window.trajectoryContext.trajectoryId,
                annotationJson: JSON.stringify(a),
                annotationJsonId: a.id
            })
        }
        await fetch(url, params);
}
btnClear.addEventListener("click", function (e){
    const annotations = anno.getAnnotations();
    if (annotations.length > 0) {
        annotations.forEach(a =>
        {
            const creatorId = getCreatorId(a);
            if (creatorId == `user:${user.userId}`) {
                anno.removeAnnotation(a);
            }

        });
       
        
    }
})

function getCreatorId(annotation) {
    return annotation.body
        ?.find(b => b.purpose === "commenting")
        ?.creator?.id;
}