// OpenGL.cpp: главный файл проекта.

#include "stdafx.h"
#include <cstdlib>

using namespace System;
using namespace PacManLogic;
using namespace Heroes;
using namespace Helpers;
using namespace GameField;

#include "glew.h" // System and OpenGL Stuff
#include "glut.h"
#include <math.h>

const int RectSize = 20;

void FillRectangle(int x, int y, int width, int height)
{
	glRecti(x, y, width-x, height-y);
}

void FillEllipse(int x, int y, int width, int height)
{
	GLfloat ox = x + (GLfloat)width/2;
	GLfloat oy = y + (GLfloat)height/2;
	auto radius = (GLfloat)width/2;
	float angle = 0;
	for (; angle <= 3.141592654; angle += 0.1)
	{
		glVertex2i(ox + radius * sin(angle), oy + radius * cos(angle));
	}
}

void FillPie(int x, int y, int width, int height, int startAngle, int totalAngle)
{
	GLfloat ox = x + (GLfloat)width/2;
	GLfloat oy = y + (GLfloat)height/2;
	auto radius = (GLfloat)width/2;
	
	for (float angle = startAngle; angle <= totalAngle - startAngle; angle += 0.1)
	{
		glVertex2i(ox + radius * sin(angle), oy + radius * cos(angle));
	}
}

 void DrawDot(Point position)
{
	glColor3ub(255, 255, 0);
    FillEllipse(RectSize * position.X + (RectSize / 2 - 2), RectSize * position.Y + (RectSize / 2 - 2), 2, 2);
}
void DrawEnergizer(Point position)
{
	glColor3ub(255, 255, 0);
    FillEllipse(RectSize * position.X, RectSize * position.Y, RectSize - 2, RectSize - 2);
}
void DrawWall(Point position)
{
	glColor3ub(0, 0, 255);
    FillRectangle(RectSize * position.X, RectSize * position.Y, RectSize, RectSize);
}
void DrawRect(Point position)
{
	glColor3ub(0, 0, 0);
    FillRectangle(RectSize * position.X, RectSize * position.Y, RectSize, RectSize);
}
void DrawField()
{
    for (int j = Field::TopEdge; j <= Field::BottomEdge; j++)
    {
        for (int i = Field::LeftEdge; i <= Field::RightEdge; i++)
        {
            DrawRect(Point(i, j));
            if (Field::Map[i, j] == PointContent::Wall)
                DrawWall(Point(i, j));
            else if (Field::Map[i, j] == PointContent::Dot)
                DrawDot(Point(i, j));
            else if (Field::Map[i, j] == PointContent::Energizer)
                DrawEnergizer(Point(i, j));
        }
    }
}
void DrawPacman(Point position, Direction direction)
{
	static Direction oldDirection;
    auto p = Point(RectSize * position.X, RectSize * position.Y);
    glColor3ub(255, 255, 0);
	FillEllipse(p.X, p.Y, RectSize, RectSize);
    int startAngle = 0;
    switch (direction)
    {
	case Direction::Up: startAngle = -120; break;
	case Direction::Down: startAngle = 60; break;
	case Direction::Left: startAngle = 150; break;
	case Direction::Right: startAngle = -30; break;
    }
	glColor4ub(0, 0, 0, 0);
    if (direction != Direction::Stop)
        FillPie(p.X, p.Y, 20, 20, startAngle, 60);
   
}
enum GhostColor
{
	Red, Pink, Blue, Orange
};
void DrawGhost(GhostColor color, Ghost ^ghost)
{
    if (Game::GhostsMode == Mode::Chase)
        switch (color)
        {
		case GhostColor::Blue:
            glColor3ub(173, 216, 230);
		case GhostColor::Red:
            glColor3ub(255, 0, 0); 
		case GhostColor::Orange:
            glColor3ub(255, 128, 0);
		case GhostColor::Pink:
			glColor3ub(255, 192, 203);
        }
    else
    {
        glColor3ub(0, 0, 255);
    }

    auto p = ghost->Position;
    
    FillEllipse(p.X, p.Y, RectSize, RectSize);
}
void RenderField()
{
    DrawField();
    DrawPacman((Game::Pacman)->Position, (Game::Pacman)->Direction);
    DrawGhost(GhostColor::Red, Game::Ghosts[0]);
    DrawGhost(GhostColor::Pink, Game::Ghosts[1]);
    DrawGhost(GhostColor::Blue, Game::Ghosts[2]);
    DrawGhost(GhostColor::Orange, Game::Ghosts[3]);
}
void RenderScene(void)
{
	RenderField();
	glutSwapBuffers();
}

void SetupRC()
{
	glEnable(GL_DEPTH_TEST); // Hidden surface removal
	glFrontFace(GL_CCW); // Counter clock-wise polygons face out
	glEnable(GL_CULL_FACE); // Do not calculate inside of jet
	// Black background
	glClearColor(0, 0, 0, 1 );
}
void SpecialKeys(int key, int x, int y)
{
	if (Game::Status == GameStatus::Running)
	{
		if(key == GLUT_KEY_UP)
			Game::TurnPacman(Direction::Up);
		if(key == GLUT_KEY_DOWN)
			Game::TurnPacman(Direction::Down);
		if(key == GLUT_KEY_LEFT)
			Game::TurnPacman(Direction::Left);
		if(key == GLUT_KEY_RIGHT)
			Game::TurnPacman(Direction::Right);
	}
	if (Game::Status != GameStatus::Running)
	{
		if (key == GLUT_KEY_F2)
			exit(0);
		if (key == GLUT_KEY_F1)
			Game::Init();
	}
	//glutPostRedisplay();
}
void TimerFunc(int value)
{
	glutPostRedisplay();
	glutTimerFunc(10, TimerFunc, 1);
}
void ChangeSize(int w, int h)
{
	GLfloat range = 210;
	if(h == 0)
		h = 1;
	glViewport(0, 0, w, h);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	// Establish clipping volume (left, right, bottom, top, near, far)
	if (w <= h)
		glOrtho (-range, range, range*h/w, -range*h/w, -1,1);
	else
		glOrtho (-range*w/h, range*w/h, range, -range, -1,1);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
}
int main(int argc, char* argv[])
{
	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB);
	glutInitWindowSize(440, 490);
	glutCreateWindow("Pacman");
	glutReshapeFunc(ChangeSize);
	glutSpecialFunc(SpecialKeys);
	glutDisplayFunc(RenderScene);
	glutTimerFunc(10, TimerFunc, 1);
	SetupRC();
	Game::Init();
	glutMainLoop();
	return 0;
}
