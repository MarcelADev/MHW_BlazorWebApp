window.colorScheme = {
    apply: function (className) {
        document.documentElement.classList.remove("dark-mode", "light-mode");
        document.documentElement.classList.add(className);
    }
};