let mouseDownHandler = null;
let keyDownHandler = null;
let index = 0;

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

export function changeFocusedElement(singleSelectElement, idx, optionId) {

    console.log("changeFocusedElement aufgerufen!");

    if (!singleSelectElement || !(singleSelectElement instanceof Element)) {
        console.warn("changeFocusedElement: invalid singleSelectElement");
        return;
    }

    const dropdown = singleSelectElement.querySelector(".single-select__dropdown");
    if (!dropdown) {
        console.warn("changeFocusedElement: .single-select__dropdown not found");
        return;
    }

    const selector = `#${optionId}-${idx}`;
    const label = dropdown.querySelector(selector);

    if (label && typeof label.focus === "function") {
        label.focus();
    } else {
        console.warn("Label not found for selector:", selector);
    }
}