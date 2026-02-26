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

    try {
        const res = await fetch(url, params);
        if (!res.ok) {
            const text = await res.text().catch(() => null);
            console.error(`Server returned ${res.status} for ${url}`, text);
            return { ok: false, status: res.status, body: text };
        }
        const json = await res.json().catch(() => null);
        return { ok: true, status: res.status, body: json };
    } catch (err) {
        console.error('Failed sending annotation request', err);
        return { ok: false, error: err };
    }
}

btnClear && btnClear.addEventListener("click", async function (e) {
    if (!anno) return;

    const annotations = anno.getAnnotations();
    if (annotations.length > 0) {
        for (const a of annotations) {
            const creatorId = getCreatorId(a);

            if (creatorId === `user:${user.userId}`) {

                const result = await sendAnnotationRequest('/Track/DeleteTrackAnnotation', a);

                if (result.ok) {
                    anno.removeAnnotation(a);
                } else {
                    console.warn('Server delete failed, removing locally anyway', result);
                    anno.removeAnnotation(a);
                }

             
            }
        }
    }
});

function getCreatorId(annotation) {
    return annotation.body
        ?.find(b => b.purpose === "commenting")
        ?.creator?.id;
}