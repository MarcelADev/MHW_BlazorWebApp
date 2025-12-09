window.screenSize = {
    initialize: function (dotnetObj) {
        function report() {
            dotnetObj.invokeMethodAsync("UpdateWidth", window.innerWidth);
        }

        // Initial call
        report();

        // Listen to resize
        window.addEventListener("resize", report);
    }
}