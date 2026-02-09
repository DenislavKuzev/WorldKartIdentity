window.process = window.process || { env: { NODE_ENV: "production" } }

import * as P from "../lib/pintura/pintura.js";


//pintura
P.setPlugins(P.plugin_crop, P.plugin_annotate);

const editor = P.appendEditor(".pnt-editor", {
    src: "/img/kustendiltrack.png",

    imageReader: P.createDefaultImageReader(),
    imageWriter: P.createDefaultImageWriter(),

    ...P.markup_editor_defaults,
    shapePreprocessor: P.createDefaultShapePreprocessor(),

    // keep the needed tools
    markupEditorToolbar: P.createMarkupEditorToolbar([
        "sharpie",
        "arrow",
        "rectangle",
        "eraser",
        "path"
    ]),
    locale: {
        ...P.locale_en_gb,
        ...P.plugin_crop_locale_en_gb,
        ...P.plugin_annotate_locale_en_gb,
        ...P.markup_editor_locale_en_gb
    }
});

let anno = null;
const loggedUserRes = await fetch('/user/me');
const loggedUser = await loggedUserRes.json();
console.log(loggedUser);
//on done editing
editor.on("process", async (result) => {
    console.log(loggedUser.authenticated);
    if (loggedUser.authenticated) {

        const blob = result.dest; //proccessed image(as blob)
        const url = URL.createObjectURL(blob);

        //close editor
        editor.destroy();

        //swap image in DOM
        const imageContainer = document.querySelector(".img-container");
        const img = document.createElement("img");
        imageContainer.innerHTML = "";      // clear first
        imageContainer.appendChild(img);    // then appen

        img.src = url;
        img.onload = () => {
            // destroy previous annotator if you re-run
            if (anno?.destroy) anno.destroy();
            anno = null;

            // v3 API
            anno = Annotorious.init({
                image: img
            });
            anno.setAuthInfo({
                id: `user:${loggedUser.userId}`,
                displayName: loggedUser.username
            });
            URL.revokeObjectURL(url);

            //attachEvents();
        };
    }
    else
    {
        window.location.href = '/User/Login';
    }
    
});



function attachEvents() {
    anno.on('createAnnotation', async (a) => {
        console.log(`reached event: ${a}, track: ${window.trackContext}`);
        const params = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                trackId: window.trackContext.trackId,
                annotationJson: JSON.stringify(a),
            })
        }

        const res = await fetch('/Track/CreateTrackAnnotation', params);
    });
}
