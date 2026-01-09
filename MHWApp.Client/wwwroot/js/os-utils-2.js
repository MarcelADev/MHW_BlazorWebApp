export function getOperatingSystem2() {
    if (navigator.userAgentData?.platform) {
        const p = navigator.userAgentData.platform;
        if (p.includes('Windows')) return 'Windows';
        if (p.includes('Mac')) return 'macOS';
        if (p.includes('Linux')) return 'Linux';
        if (p.includes('Android')) return 'Android';
        if (p.includes('iOS')) return 'iOS';
    }

    const ua = navigator.userAgent;
    if (/Windows/.test(ua)) return 'Windows';
    if (/Mac/.test(ua)) return 'macOS';
    if (/Linux/.test(ua)) return 'Linux';
    if (/Android/.test(ua)) return 'Android';
    if (/iPhone|iPad/.test(ua)) return 'iOS';

    return 'Unknown';
}