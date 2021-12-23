let checkboxes = Array.from(document.getElementsByClassName('checkbox'));

if (checkboxes) {
    for(let checkbox of checkboxes) {
        checkbox.addEventListener('change', (f) => {

            for(let item of checkboxes) {
                if(item.id !== f.target.id) {
                    item.checked = false;
                }
            }
        });
    }
}
else {
    console.error("Can't find checkboxes");
}