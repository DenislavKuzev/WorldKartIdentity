window.process = window.process || { env: { NODE_ENV: "production" } }

import * as P from "../lib/pintura/pintura.js";
import  "../lib/node_modules/@recogito/annotorious/dist/annotorious.min.js";

//import "../img/kustendil"

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
//on done editing
editor.on("process", (result) => {

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
    img.style.maxWidth = "100%";
    img.style.height = "auto";

    img.onload = () => {
        // destroy previous annotator if you re-run
        if (anno?.destroy) anno.destroy();
        anno = null;

        // v3 API
        anno = Annotorious.init({
            image: img
        }); // or just createImageAnnotator(img) if global is in scope
        // anno.loadAnnotations('...') etc.

        URL.revokeObjectURL(url);
    };
});


