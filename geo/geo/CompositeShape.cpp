#include "base-types.h"
#include "Shape.h"
#include "Isotrapezium.h"
#include "CompositeShape.h"
#include <iostream>

double CompositeShape::getArea() {
	
	rectangle temp = rectangle(getPoints(0), getPoints(2), getPoints(1), getPoints(3));
	return temp.getArea();
};

CompositeShape& CompositeShape::clone() {
	CompositeShape* next = new CompositeShape(countOfShapes);
	for (int i = 0; i < countOfShapes; i++) {
		next->setFigure(getFigure(i).clone(), i);
	}
	next->moveTo(center_.getX(), center_.getY());
	return *next;
}

void CompositeShape::move(double delta_x, double delta_y) {
	center_.setXY(center_.getX() + delta_x, center_.getY() + delta_y);
	for (int i = 0; i < countOfShapes; i++)
	{
		if (&getFigure(i) == nullptr)
			continue;
		getFigure(i).move(delta_x, delta_y);
	}
}
void CompositeShape::moveTo(double x, double y) {
	double delta_x = x - center_.getX();
	double delta_y = y - center_.getY();
	move(delta_x, delta_y);
}

void CompositeShape::scale(double x_, double y_, double sigma) {
	double x = (x_ - getX()) * sigma;
	double y = (y_ - getY()) * sigma;
	moveTo(x, y);
	for (int i = 0; i < countOfShapes; i++)
	{
		if (&getFigure(i) == nullptr)
			continue;
		getFigure(i).scale(center_.getX(), center_.getY(), sigma);
	}
}

std::string CompositeShape::getName() {
	return "COMPOSITESHAPE";
}

rectangle& CompositeShape::getFrameRect() {
	rectangle* temp = new rectangle(getPoints(0), getPoints(2), getPoints(1), getPoints(3));
	return *temp;
}

double CompositeShape::getMaxWidth() {//unused
	return 1;
}
double CompositeShape::getMaxHeigth() {//unused
	return 1;
}

void CompositeShape::setFigure(Shape& figure, int index) {
	if (countOfShapes > index) {
		if (&figure == nullptr)
			return;
		arrayOfShapes[index] = &figure.clone();
	}
}

Shape& CompositeShape::getFigure(int index) {
	return *arrayOfShapes[index];
}

double CompositeShape::getPoints(int index) {
	double supremum[4];
	for (int i = 0; i < 4; i++)
	{
		supremum[i] = getFigure(0).getPoints(i);;
	}
	for (int i = 1; i < countOfShapes; i++)
	{
		if (&getFigure(i) == nullptr)
			continue;
		supremum[0] = getFigure(i).getPoints(0) < supremum[0] ? getFigure(i).getPoints(0) : supremum[0];
		supremum[1] = getFigure(i).getPoints(1) > supremum[1] ? getFigure(i).getPoints(1) : supremum[1];
		supremum[2] = getFigure(i).getPoints(2) < supremum[2] ? getFigure(i).getPoints(2) : supremum[2];
		supremum[3] = getFigure(i).getPoints(3) > supremum[3] ? getFigure(i).getPoints(3) : supremum[3];
	}
	return supremum[index];
}