#ifndef NAV_H
#define NAV_H

#include "navext.h"
#include "Recast.h"
#include "DetourNavMesh.h"
#include "DetourNavMeshQuery.h"
#include "InputGeom.h"

class Nav {
protected:
	InputGeom * m_geom;
	dtNavMesh* m_navMesh;
	dtNavMeshQuery* m_navQuery;
	BuildContext*  m_ctx;

	float m_cellSize;
	float m_cellHeight;
	float m_agentHeight;
	float m_agentRadius;
	float m_agentMaxClimb;
	float m_agentMaxSlope;
	float m_regionMinSize;
	float m_regionMergeSize;
	float m_edgeMaxLen;
	float m_edgeMaxError;
	float m_vertsPerPoly;
	float m_detailSampleDist;
	float m_detailSampleMaxError;
	int m_partitionType;

	bool m_filterLowHangingObstacles;
	bool m_filterLedgeSpans;
	bool m_filterWalkableLowHeightSpans;

	//SoloMesh
	bool m_keepInterResults;
	float m_totalBuildTimeMs;
	unsigned char* m_triareas;
	rcHeightfield* m_solid;
	rcCompactHeightfield* m_chf;
	rcContourSet* m_cset;
	rcPolyMesh* m_pmesh;
	rcConfig m_cfg;
	rcPolyMeshDetail* m_dmesh;
	//NavMeshTesterTool
	dtQueryFilter m_filter;
	static const int MAX_POLYS = 256;
	static const int MAX_SMOOTH = 2048;
	float m_polyPickExt[3];
	dtPolyRef m_polys[MAX_POLYS];
	int m_npolys;
	dtPolyRef m_straightPathPolys[MAX_POLYS];
	unsigned char m_straightPathFlags[MAX_POLYS];

	void defaultCommonSettings();
	void cleanup();
public:
	Nav();
	~Nav();

	void setContext(BuildContext* ctx) { m_ctx = ctx; }
	InputGeom* getInputGeom() { return m_geom; }
	dtNavMesh* getNavMesh() { return m_navMesh; }
	dtNavMeshQuery* getNavMeshQuery() { return m_navQuery; }
	float getAgentRadius() { return m_agentRadius; }
	float getAgentHeight() { return m_agentHeight; }
	float getAgentClimb() { return m_agentMaxClimb; }

	void setCommonSettings(float cellSize, float cellHeight,
		float agentHeight, float agentRadius,
		float agentMaxClimb, float agentMaxSlope);
	bool load(const char* path);
	bool build();
	bool getRandomPos(float* pos);
	bool getHeight(const float* pos,float distance,float* height);
	bool getPath(const float* spos,const float* epos,float* path,int *npath);
};

#endif // !NAV_H
