window.process = window.process || { env: { NODE_ENV: "production" } }

import * as P from "../lib/pintura/pintura.js";

const userRes = await fetch('/user/me');
const user = await userRes.json();

//pintura
if (user.authenticated) {
    console.log(window.trackContext.trackBase64);
    P.setPlugins(P.plugin_crop, P.plugin_annotate);

    

    const editor = P.appendEditor(".pnt-editor", {
        src: "data:image/png;base64," + window.trackContext.trackBase64,

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

    //on done editing
    editor.on("process", async (result) => {
        const blob = result.dest; //proccessed image(as blob)
        const imageBase64 = await blobToBase64(blob);

        const res = await fetch("/Track/CreateTrajectory", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                trackId: window.trackContext.trackId,
                base64: imageBase64
            })
        });

    });
}
async function blobToBase64(blob) {
    const arrayBuffer = await blob.arrayBuffer();
    const bytes = new Uint8Array(arrayBuffer);

    let binary = "";
    bytes.forEach(b => binary += String.fromCharCode(b));

    return btoa(binary);
}