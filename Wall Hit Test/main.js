export const createScene = async function () {
    var scene = new BABYLON.Scene(engine);

    var light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(0, 1, 0), scene);

    // Center of your circular boundary
    const circleCenter = new BABYLON.Vector3(0, 0, 0);

    const xr = await scene.createDefaultXRExperienceAsync({
        uiOptions: {
            sessionMode: "immersive-ar",
            referenceSpaceType: "local"
        },
        optionalFeatures: true,
        requiredFeatures: ["hit-test"]
    });

    const hitTest = await xr.baseExperience.featuresManager.enableFeature(
        BABYLON.WebXRHitTest,
        "latest",
        { offsetRay: new BABYLON.Vector3(0, 0, 0) }
    );

    let hitPos = new BABYLON.Vector3();

    hitTest.onHitTestResultObservable.add((results) => {
        if (results.length > 0) {
            results[0].transformationMatrix.decompose(undefined, undefined, hitPos);
        }
    });

    scene.onPointerDown = function () {
        if (!hitTest || hitPos.length() === 0) return;

        const wall = BABYLON.MeshBuilder.CreateBox(
            "wall",
            { width: 0.2, height: 1.5, depth: 1 },
            scene
        );

        wall.position.copyFrom(hitPos);
        wall.position.y += 0.75;

        // Compute direction from wall to center
        const direction = circleCenter.subtract(wall.position);

        // Standard yaw (same as before)
        const yaw = Math.atan2(direction.x, direction.z);

        // FIX: Correct Babylon/mesh forward axis (rotate 90 degrees)
        const yawCorrected = yaw + Math.PI / 2;

        wall.rotationQuaternion = BABYLON.Quaternion.FromEulerAngles(
            0,
            yawCorrected,
            0
        );

        const mat = new BABYLON.StandardMaterial("wallMat", scene);
        mat.diffuseColor = new BABYLON.Color3(0.9, 0.9, 0.9);
        wall.material = mat;
    };

    return scene;
};