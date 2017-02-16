// Fill out your copyright notice in the Description page of Project Settings.

#include "GAMEPRODEV2_QBert.h"
#include "QBertCharacter.h"


// Sets default values
AQBertCharacter::AQBertCharacter()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AQBertCharacter::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AQBertCharacter::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

// Called to bind functionality to input
void AQBertCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

