// Copyright Epic Games, Inc. All Rights Reserved.

#include "DepressionGameMode.h"
#include "DepressionCharacter.h"
#include "UObject/ConstructorHelpers.h"

ADepressionGameMode::ADepressionGameMode()
{
	// set default pawn class to our Blueprinted character
	static ConstructorHelpers::FClassFinder<APawn> PlayerPawnBPClass(TEXT("/Game/ThirdPersonCPP/Blueprints/ThirdPersonCharacter"));
	if (PlayerPawnBPClass.Class != NULL)
	{
		DefaultPawnClass = PlayerPawnBPClass.Class;
	}
}
