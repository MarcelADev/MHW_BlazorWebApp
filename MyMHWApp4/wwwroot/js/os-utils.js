function getOperatingSystem() {
    if (navigator.userAgentData) {
        const platform = navigator.userAgentData.platform || navigator.userAgentData.brands.map(brand => brand.brand).join(' ');
        if (platform.includes('Windows')) {
            return 'Windows';
        } else if (platform.includes('macOS') || platform.includes('Macintosh')) {
            return 'Mac OS';
        } else if (platform.includes('Linux')) {
            return 'Linux';
        } else if (platform.includes('Android')) {
            return 'Android';
        } else if (platform.includes('iOS') || platform.includes('iPhone') || platform.includes('iPad')) {
            return 'iOS';
        }
    } else {
        // Fallback for older browsers
        let userAgent = window.navigator.userAgent;
        let platform = window.navigator.platform;
        let macosPlatforms = ['Macintosh', 'MacIntel', 'MacPPC', 'Mac68K'];
        let windowsPlatforms = ['Win32', 'Win64', 'Windows', 'WinCE'];
        let iosPlatforms = ['iPhone', 'iPad', 'iPod'];
        let os = null;

        if (macosPlatforms.indexOf(platform) !== -1) {
            os = 'Mac OS';
        } else if (iosPlatforms.indexOf(platform) !== -1) {
            os = 'iOS';
        } else if (windowsPlatforms.indexOf(platform) !== -1) {
            os = 'Windows';
        } else if (/Android/.test(userAgent)) {
            os = 'Android';
        } else if (!os && /Linux/.test(platform)) {
            os = 'Linux';
        }

        return os;
    }
}