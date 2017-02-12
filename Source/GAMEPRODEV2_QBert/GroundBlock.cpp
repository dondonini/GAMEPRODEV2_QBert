// Fill out your copyright notice in the Description page of Project Settings.

#include "GAMEPRODEV2_QBert.h"
#include "GroundBlock.h"


// Sets default values
AGroundBlock::AGroundBlock()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	m_BlockRoot = CreateDefaultSubobject<USceneComponent>(TEXT("BlockRoot"));
	RootComponent = m_BlockRoot;

	m_BlockMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("BlockMesh"));
	m_BlockMesh->AttachToComponent(m_BlockRoot, FAttachmentTransformRules::SnapToTargetNotIncludingScale);

	m_BlockTopColor = CreateDefaultSubobject<UDecalComponent>(TEXT("BlockTopFace"));
	m_BlockTopColor->AttachToComponent(m_BlockRoot, FAttachmentTransformRules::SnapToTargetIncludingScale);

	//m_BlockCollider = CreateDefaultSubobject<UShapeComponent>(TEXT("BlockBox"));
	//m_BlockCollider->SetWorldScale3D(FVector(1.0f, 1.0f, 1.0f));

}

// Called when the game starts or when spawned
void AGroundBlock::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AGroundBlock::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

