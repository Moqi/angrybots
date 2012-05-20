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
