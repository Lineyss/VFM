export const countingChars = (input) => {
    const parent = input.parentElement;
    const span = parent.querySelector("snap");
    span.textContent = `${input.value.length}/${input.maxLength}`
}