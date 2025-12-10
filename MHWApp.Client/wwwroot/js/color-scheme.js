window.colorScheme = {
    apply: function (className) {
        document.body.classList.remove("dark-mode", "light-mode");
        document.body.classList.add(className);
    }
};