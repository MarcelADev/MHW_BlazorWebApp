let mouseDownHandler = null;

export function addMouseClickListener(singleSelectElement, dotNetHelper) {
    if (mouseDownHandler) return;
    console.log("MouseClickListener added");

    mouseDownHandler = (event) => {
        console.log("Mouse down event detected");
        if (!singleSelectElement.contains(event.target)) {
            dotNetHelper.invokeMethodAsync("HandleOutsideClick");
        }
    };

    document.addEventListener("mousedown", mouseDownHandler, true);
}

export function removeMouseClickListener() {
    if (!mouseDownHandler) return;
    console.log("MouseClickListener removed");

    document.removeEventListener("mousedown", mouseDownHandler, true);
    mouseDownHandler = null;
}