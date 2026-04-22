function applyControlStyles() {
    controlElement = Array.from(
        document.querySelectorAll("fluent-dialog")
    ).find(
        node => Array.from(
            node.shadowRoot.querySelector(".control").firstElementChild.assignedNodes()
        ).find(
            n => (n.className === "fluent-dialog-body" && n.firstElementChild.className === "image-viewer")
        ) != undefined
    ).shadowRoot.querySelector(".control");

    controlElement.style.setProperty("height", "fit-content", "important");
    controlElement.style.setProperty("width", "fit-content", "important");
}
