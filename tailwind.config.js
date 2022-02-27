module.exports = {
    mode: "jit",
    purge: {
        content: ["./Components/**/*.razor", "./Pages/**/*.razor", "./Shared/**/*.razor"],
        options: {
            safelist: [
                /data-theme$/,
            ]
        },
    },
    darkMode: true,
    theme: {
        extend: {

        },
    },
    variants: {
        extend: {

        },
    },
    plugins: [
        require('daisyui'),
        require('@tailwindcss/typography'),
    ],
}