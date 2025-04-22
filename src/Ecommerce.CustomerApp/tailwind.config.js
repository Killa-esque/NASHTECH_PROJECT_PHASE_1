/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.cshtml",
    "./Views/**/*.cshtml",
    "./Views/Shared/**/*.cshtml",
  ],
  darkMode: "class",
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
};
