 //Save material as json from js to program and send with modeldata
var material = new THREE.MeshStandardMaterial();

var scene = new THREE.Scene();

var renderer = new THREE.WebGLRenderer({alpha:true});
renderer.setClearColor(0x000000, 0);
var camera = new THREE.PerspectiveCamera();
camera.position.z = 4;
camera.lookAt(0, 0, 0);

var light = new THREE.DirectionalLight();
light.shadow.camera.left = -20;
light.shadow.camera.right = 20;
light.shadow.camera.top = 20;
light.shadow.camera.bottom = -20;

var ambient = new THREE.AmbientLight();
ambient.intensity = 0.5;
light.intensity = 0.8;

var sphere = new THREE.Mesh(new THREE.SphereGeometry(1, 32, 32), material);

scene.add(sphere, light, ambient, camera);

function setFloat(float, type) {

    if (type == "rotation") {
        sunSpeed = float;
        var d = sunSpeed / 180;
        var rad = d * Math.PI;
        light.position.set(Math.cos(rad), 0, Math.sin(rad));
    }
    else if (type == "intensity") {
        light.intensity = float;
    }
    else if (type == "normalScale") {
        material[type] = new THREE.Vector2(float, float);
        material.needsUpdate = true;
    }
    else {
        material[type] = float;
        material.needsUpdate = true;
    }

}

function setTiling(x, y) {

    var maps = [material.map, material.normalMap, material.roughnessMap, material.displacementMap];

    material.map.tex

    for (var m = 0; m < maps.length; m++) {

        if (maps[m]) {
             maps[m].repeat.set(x, y)
        }
    }

    material.needsUpdate = true;

}

function setTexture(json, type) {
    console.log(json, type)
    var img = 'data:image/png;base64,' + json;
    material[type] = THREE.ImageUtils.loadTexture(img);
    material[type].wrapS = material[type].wrapT = THREE.RepeatWrapping;
    material.needsUpdate = true;

    console.log(material[type]);
    //material.map = new THREE.TextureLoader().load(path);
}




var sunSpeed = -1;

function render() {

    if (sunSpeed == -1) {
        var p = performance.now() / 2000;
        light.position.set(Math.cos(p), 0, Math.sin(p));
    }


    renderer.render(scene, camera);
    requestAnimationFrame(render);
}


function run(elem) {

    console.log("running...")
    renderer.setSize(elem.clientWidth, elem.clientHeight);

    elem.appendChild(renderer.domElement);
    render();
}

window.onload = function () {

    //$("input").change(function (event) {
    //    console.log("changing..")

    //    var path = event.currentTarget.value;


    //    var blob = event.currentTarget.files[0];

    //    var reader = new FileReader();

    //    reader.onload = function (event) {

    //        scriptManager.setImage(event.target.result);
    //    }

    //    reader.readAsBinaryString(blob);



    //})



    run(document.getElementById("render-view"));
}