export function toggleOverflowVisibility(isOpen, wrapperId) {
    const wrapper = document.getElementById(wrapperId);

    if (!wrapper) { return; }

    if (isOpen) {
        const onEnd = (e) => {
            if (e.target === wrapper && e.propertyName === "grid-template-rows") {
                wrapper.classList.add("overflow-visible");
                wrapper.removeEventListener("transitionend", onEnd);
            }
        };

        wrapper.addEventListener("transitionend", onEnd);
    }
    else {
        wrapper.classList.remove("overflow-visible");
    }
}