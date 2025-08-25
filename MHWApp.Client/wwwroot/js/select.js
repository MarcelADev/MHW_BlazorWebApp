export const addLabelHandlers = (componentId) => {
    const label = document.getElementById(`${componentId}`);

    if (!label) return;

    label.addEventListener("click", (_) => {
        const targetId = label.getAttribute("for");

        if (!targetId) return;

        const target = document.getElementById(targetId).firstChild;
        target.focus();
    });
}