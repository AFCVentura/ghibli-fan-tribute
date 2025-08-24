module.exports = {
    content: [
        "./Pages/**/*.{html,js,razor,cshtml}",
        "./Components/**/*.{html,js,razor,cshtml}",
        "./Shared/**/*.{html,js,razor,cshtml}",
        "./**/*.{html,js,razor,cshtml}",
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    DEFAULT: '#109CEB', // Blue
                    light: '#3B82F6', // Light Blue
                    dark: '#1E3A8A', // Dark Blue
                },
            },
        },
    },
    plugins: [],
}