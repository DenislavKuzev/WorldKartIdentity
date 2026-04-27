// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//setTimeout(() => {
//    const dialog = document.querySelector('.result-dialog');
//    if (dialog) {
//        dialog.close();
//    }
//}, 4000);
tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            "colors": {
                "on-tertiary-fixed-variant": "#005142",
                "on-error-container": "#ffdad6",
                "on-tertiary-fixed": "#002019",
                "background": "#0d1323",
                "on-tertiary-container": "#eefff7",
                "surface": "#0d1323",
                "surface-container-high": "#24293b",
                "on-secondary": "#233143",
                "error-container": "#93000a",
                "on-secondary-fixed": "#0d1c2d",
                "surface-tint": "#ffb3b6",
                "on-surface-variant": "#e5bdbe",
                "surface-container-highest": "#2f3446",
                "outline-variant": "#5c3f40",
                "surface-bright": "#33394a",
                "on-error": "#690005",
                "surface-container": "#191f30",
                "on-primary-fixed": "#40000c",
                "secondary-fixed": "#d4e4fa",
                "primary-fixed": "#ffdada",
                "on-secondary-container": "#a7b6cc",
                "error": "#ffb4ab",
                "inverse-surface": "#dde2f9",
                "on-tertiary": "#00382d",
                "tertiary-fixed-dim": "#74d8bd",
                "primary-fixed-dim": "#ffb3b6",
                "surface-dim": "#0d1323",
                "on-surface": "#dde2f9",
                "tertiary-container": "#00836c",
                "on-secondary-fixed-variant": "#39485a",
                "on-background": "#dde2f9",
                "inverse-on-surface": "#2a3041",
                "surface-variant": "#2f3446",
                "primary-container": "#e11d48",
                "on-primary-fixed-variant": "#920028",
                "secondary-fixed-dim": "#b9c8de",
                "tertiary": "#74d8bd",
                "surface-container-low": "#151b2c",
                "outline": "#ac8889",
                "secondary-container": "#39485a",
                "secondary": "#b9c8de",
                "on-primary-container": "#fffaf9",
                "surface-container-lowest": "#080e1d",
                "on-primary": "#68001a",
                "inverse-primary": "#be0037",
                "tertiary-fixed": "#90f5d9",
                "primary": "#ffb3b6"
            },
            "borderRadius": {
                "DEFAULT": "0.25rem",
                "lg": "0.5rem",
                "xl": "0.75rem",
                "full": "9999px"
            },
            "fontFamily": {
                "headline": ["Space Grotesk"],
                "body": ["Inter"],
                "label": ["Inter"]
            }
        },
    },
}