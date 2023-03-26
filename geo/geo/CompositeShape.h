#pragma once
#ifndef COMPOSITE
#define COMPOSITE
#include "base-types.h"
#include "Shape.h"
#include "Isotrapezium.h"


class CompositeShape : public Shape {
public:
	CompositeShape(int count) { countOfShapes = count; arrayOfShapes = new Shape * [countOfShapes]; };
	~CompositeShape() {
		for (int i = 0; i < countOfShapes; i++) {delete arrayOfShapes[i];} delete[] arrayOfShapes;
	};
	double getArea();
	CompositeShape& clone();
	rectangle& getFrameRect();
	void move(double delta_x, double delta_y);
	void moveTo(double x, double y);
	void scale(double x, double y, double sigma);
	std::string getName();
	double getMaxWidth();
	double getMaxHeigth();
	void setFigure(Shape& figure, int index);
	Shape& getFigure(int index);
	double getPoints(int index);
	double getX() { return center_.getX(); };
	double getY() { return center_.getY(); };
private:
	int countOfShapes;
	Shape** arrayOfShapes;
	point_t center_;
};


#endif // !COMPOSITE
