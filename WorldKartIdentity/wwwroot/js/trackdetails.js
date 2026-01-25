import * as P from "../lib/pintura/pintura.js";
import * as A from "../lib/annotorious/annotorious.min.js";
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
        "eraser"
    ]),
    locale: {
        ...P.locale_en_gb,
        ...P.plugin_crop_locale_en_gb,
        ...P.plugin_annotate_locale_en_gb,
        ...P.markup_editor_locale_en_gb
    }
});

let annoEditor = null;
//on done editing
editor.on("process", (result) => {

    const blob = result.dest; //proccessed image(as blob)
    const url = URL.createObjectURL(blob);

    //close editor
    editor.destroy();

    //swap image in DOM
    const img = document.getElementById("photo");
    const imageContainer = document.querySelector(".imageContainer");
    img.src = url;
    img.onload = () => {
        image
        annoEditor = A.Annotorious.init({
            image: ''
        })
        URL.revokeObjectURL(url);
    };
});


