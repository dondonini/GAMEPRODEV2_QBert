// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Actor.h"
#include "GroundBlock.generated.h"

UCLASS()
class GAMEPRODEV2_QBERT_API AGroundBlock : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AGroundBlock();

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	// Called every frame
	virtual void Tick( float DeltaSeconds ) override;

	UPROPERTY(EditAnywhere)
		USceneComponent* m_BlockRoot;

	UPROPERTY(EditAnywhere)
		UStaticMeshComponent* m_BlockMesh;

	UPROPERTY(EditAnywhere)
		UShapeComponent* m_BlockCollider;
	
};
