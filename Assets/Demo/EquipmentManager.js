#pragma strict


private var equipMap = {};

function Reset() {
	equipMap = {};
}

function IsEquipped(equipmentId) {
	return equipMap[equipmentId] == true;
}

function Equip(equipmentId) {
	equipMap[equipmentId] = true;
}

function Unequip(equipmentId) {
	equipMap[equipmentId] = false;
}

function Use(equipmentId) {
	if(equipmentId == 'super_speed') {
		GameObject.FindWithTag ("Player").GetComponent(FreeMovementMotor).walkingSpeed = 11;
	}
}
