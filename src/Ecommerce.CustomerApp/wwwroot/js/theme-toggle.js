const html = document.documentElement;
const toggleBtn = document.getElementById("toggleDarkMode");

// Auto apply theme on load
if (localStorage.theme === "dark") {
  html.classList.add("dark");
} else {
  html.classList.remove("dark");
}

// Toggle theme
toggleBtn?.addEventListener("click", () => {
  html.classList.toggle("dark");
  localStorage.theme = html.classList.contains("dark") ? "dark" : "light";
});
